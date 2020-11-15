using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTalk
{
    public class MainWindow : Window
    {
        private readonly ListBox lstContacts;
        private readonly ListBox lstMessages;
        private readonly TextBox tbMessage;
        private readonly Button btnSendMessage;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            lstContacts = this.FindControl<ListBox>("lstContacts");
            lstMessages = this.FindControl<ListBox>("lstMessages");
            tbMessage = this.FindControl<TextBox>("tbMessage");
            btnSendMessage = this.FindControl<Button>("btnSendMessage");

            tbMessage.GotFocus += (s, e) =>
            {
                if (tbMessage.Text == "message...")
                {
                    tbMessage.Text = string.Empty;
                    tbMessage.Foreground = Avalonia.Media.Brushes.Black;
                }
            }; // remove placeholder text
            tbMessage.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(tbMessage.Text))
                {
                    tbMessage.Text = "message...";
                    tbMessage.Foreground = Avalonia.Media.Brushes.LightGray;
                }
            }; // replace placeholder text
            tbMessage.KeyUp += (s, e) => btnSendMessage.IsEnabled = !string.IsNullOrEmpty(tbMessage.Text);
            btnSendMessage.Click += BtnSendMessage_Click;
            lstContacts.SelectionChanged += LstContacts_SelectionChanged;

            // make sure we have 318-677-733
            Task.Run(async () =>
            {
                static byte[] GetBytes(string s) => System.Text.Encoding.UTF8.GetBytes(s);
                using DataContext db = new();
                if (db.Contacts.Any(x => x.UserId == "318-677-733"))
                    return;
                ClientModels.Users.User user = await APIClient.v1.Users.Get("318-677-733");
                Models.MessageKey key = new()
                {
                    AssignedContactId = user.Id,
                    Created = DateTime.UtcNow,
                    PublicPem = user.ExchangePem,
                    Fingerprint = user.ExchangeFingerprint,
                    IsExchange = true
                };
                Models.Contact testContact = new()
                {
                    Name = "Test",
                    ExchangeFingerprint = user.ExchangeFingerprint,
                    UserId = user.Id,
                };
                db.Contacts.Add(testContact);
                await db.SaveChangesAsync();
                db.MessageKeys.Add(key);
                await db.SaveChangesAsync();
                // add a message so that it shows up
                byte[] text = GetBytes("Hello, world!");
                AsymmetricKeyParameter sendKey = CryptoService.ImportKey(key.PublicPem);
                AsymmetricCipherKeyPair ourKeyPair = CryptoService.GenerateKeypair(4096);
                Models.MessageKey ourKeyMdl = new()
                {
                    AssignedContactId = testContact.UserId,
                    Fingerprint = CryptoService.Fingerprint(ourKeyPair),
                    PrivatePem = CryptoService.ExportKeypair(ourKeyPair, CryptoService.KeyType.Private),
                    PublicPem = CryptoService.ExportKeypair(ourKeyPair, CryptoService.KeyType.Public)
                };
                db.MessageKeys.Add(ourKeyMdl);
                AsymmetricKeyParameter ourKey = CryptoService.ImportKey(ourKeyMdl.PublicPem);
                Models.Message testMessage = new()
                {
                    DecryptedRecipientId = testContact.UserId,
                    DecryptedSenderId = Services.API.UserId,
                    KeyFingerprint = key.Fingerprint,
                    Sent = DateTime.UtcNow,
                    MessageText = CryptoService.Encrypt(text, sendKey),
                    SenderText = CryptoService.Encrypt(text, ourKey),
                    RecipientId = CryptoService.Encrypt(GetBytes(testContact.UserId), sendKey),
                    SenderId = CryptoService.Encrypt(GetBytes(Services.API.UserId), sendKey),
                    SenderFingerprint = CryptoService.Fingerprint((RsaKeyParameters)ourKey)
                };
                db.Messages.Add(testMessage);
                await db.SaveChangesAsync();

            }).ContinueWith(t =>
            {
                using DataContext db = new();
                List<string> messageContacts = db.Messages.Where(x => x.DecryptedSenderId == Services.API.UserId).Select(x => x.DecryptedRecipientId).Distinct().ToList();
                messageContacts.AddRange(db.Messages.Where(x => x.DecryptedRecipientId == Services.API.UserId).Select(x => x.DecryptedSenderId).Distinct());
                lstContacts.Items = db.Contacts.Where(x => messageContacts.Distinct().Contains(x.UserId)).ToArray();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void BtnSendMessage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            btnSendMessage.IsEnabled = false;
            string msgtext = tbMessage.Text;
            tbMessage.Text = string.Empty;
            Task.Run(async () =>
            {
                Models.Contact sendTo = lstContacts.SelectedItem as Models.Contact;
                using DataContext db = new();
                // get key for that contact
                Models.MessageKey key = db.MessageKeys
                    .Include(x => x.Contact)
                    .FirstOrDefault(x => x.Contact.UserId == sendTo.UserId && x.AssignedContactId == sendTo.UserId);
                Models.MessageKey myKey = db.MessageKeys
                    .Include(x => x.Contact)
                    .FirstOrDefault(x => x.AssignedContactId == sendTo.UserId && !string.IsNullOrEmpty(x.PrivatePem));
                AsymmetricKeyParameter pubKey = CryptoService.ImportKey(key.PublicPem);
                AsymmetricKeyParameter myPub = CryptoService.ImportKey(myKey.PublicPem);
                byte[] messagebytes = System.Text.Encoding.UTF8.GetBytes(msgtext);
                Models.Message message = new()
                {
                    KeyFingerprint = key.Fingerprint,
                    MessageText = CryptoService.Encrypt(messagebytes, pubKey),
                    SenderId = CryptoService.Encrypt(System.Text.Encoding.UTF8.GetBytes(Services.API.UserId), pubKey),
                    RecipientId = CryptoService.Encrypt(System.Text.Encoding.UTF8.GetBytes(sendTo.UserId), pubKey),
                    // server side tracking
                    SenderFingerprint = myKey.Fingerprint,
                    SenderText = CryptoService.Encrypt(messagebytes, myPub),
                    // calculated later
                    DecryptedSenderId = Services.API.UserId,
                    DecryptedRecipientId = sendTo.UserId,
                    Sent = DateTime.UtcNow
                };
                db.Messages.Add(message);
                await db.SaveChangesAsync();
                // todo: the message sent to the server is above.
                // the one saved to DB needs to be encrypted with
                // our pubkey for that person
                return new ViewModels.DecryptedMessage
                {
                    IsMe = true,
                    DateUtc = message.Sent,
                    Message = msgtext
                };
            }).ContinueWith(async t =>
            {
                if (t.IsFaulted)
                    Console.WriteLine("ERR");
                List<ViewModels.DecryptedMessage> mdl = new();
                mdl.AddRange((ViewModels.DecryptedMessage[])(lstMessages.Items ?? Array.Empty<ViewModels.DecryptedMessage>()));
                mdl.Add(await t);
                lstMessages.Items = mdl.ToArray();
                tbMessage.Focus();
                btnSendMessage.Focus();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LstContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstMessages.Items = null;
            Task.Run(() =>
            {
                using DataContext db = new();
                (string fingerprint, AsymmetricKeyParameter key) lastKey = new();
                Models.Contact contact = lstContacts.SelectedItem as Models.Contact;
                return db.Messages
                    .Where(x => (x.DecryptedRecipientId == Services.API.UserId && x.DecryptedSenderId == contact.UserId) ||
                                (x.DecryptedRecipientId == contact.UserId && x.DecryptedSenderId == Services.API.UserId))
                    .OrderBy(x => x.Sent)
                    .AsEnumerable()
                    .Select(msg =>
                    {
                        if ((msg.SenderFingerprint ?? msg.KeyFingerprint) is string kf && lastKey.fingerprint != kf)
                            lastKey.key = CryptoService.ImportKey(db.MessageKeys.Find(kf).PrivatePem);
                        ViewModels.DecryptedMessage m = new()
                        {
                            DateUtc = msg.Sent,
                            IsMe = msg.DecryptedSenderId == Services.API.UserId,
                            Message = System.Text.Encoding.UTF8.GetString(CryptoService.Decrypt(msg.SenderText ?? msg.MessageText, lastKey.key))
                        };
                        return m;
                    }).ToArray();
            }).ContinueWith(async t => lstMessages.Items = await t, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}

using Org.BouncyCastle.Crypto;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTalk.Services
{
    internal static class API
    {
        static async Task CreateKeypairIfNotExist()
        {
            string KeyPath = @"C:\Users\ethan\keypair";
            using DataContext db = new();
            if ((App.ActiveUser = db.Users.FirstOrDefault(x => x.Active)) != null)
                return;
            AsymmetricCipherKeyPair pair = CryptoService.GenerateKeypair(512);
            string key = CryptoService.ExportKeypair(pair, CryptoService.KeyType.Private);
            File.WriteAllText(KeyPath, key);
            key = CryptoService.ExportKeypair(pair, CryptoService.KeyType.Public);
            File.WriteAllText($"{KeyPath}.pub", key);
            ClientModels.Users.User user = await APIClient.v1.Users.Register(new ClientModels.Users.CreateModel
            {
                ExchangePem = key
            });
            Models.User createdUser = Models.User.FromBase(ref user);
            createdUser.Active = true;
            createdUser.PEMPath = KeyPath;
            db.Users.Add(createdUser);
            await db.SaveChangesAsync();
            App.ActiveUser = createdUser;
        }
        internal static AsymmetricKeyParameter PrivateKey => CryptoService.ImportKey(File.ReadAllText(App.ActiveUser.PEMPath));
        internal static AsymmetricKeyParameter PublicKey => CryptoService.ImportKey(File.ReadAllText($"{App.ActiveUser.PEMPath}.pub"));
        internal static string UserId => App.ActiveUser.Id;
        public static void Configure()
        {
            Task.Run(CreateKeypairIfNotExist).Wait();
            APIClient.Settings.GetPrivateKey = () => PrivateKey;
            APIClient.Settings.GetPublicKey = () => PublicKey;
            APIClient.Settings.GetUserId = () => UserId;
        }
    }
}

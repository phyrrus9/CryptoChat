using System;

namespace SecureTalk.ViewModels
{
    public class DecryptedMessage
    {
        public bool IsMe { get; set; }
        public string Alignment => IsMe ? "Right" : "Left";
        public DateTime DateUtc { get; set; }
        private int UTZ => (DateTime.Now - DateTime.UtcNow).Hours;
        public string Date => DateUtc.AddHours(UTZ).ToString();
        public string Message { get; set; }
    }
}

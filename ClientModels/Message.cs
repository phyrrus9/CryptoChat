using System;
using System.ComponentModel.DataAnnotations;

namespace ClientModels
{
    public class Message
    {
        [Key] public Guid Id { get; set; }
        [Required] public string KeyFingerprint { get; set; }
        [Required] public byte[] SenderId { get; set; } // encrypted
        [Required] public byte[] RecipientId { get; set; } // encrypted
        [Required] public byte[] MessageText { get; set; } // encrypted
        public DateTime Sent { get; set; } // plaintext, set by server

        public string SenderFingerprint { get; set; } // for server side tracking
        public byte[] SenderText { get; set; } // encrypted, for server side tracking
    }
}

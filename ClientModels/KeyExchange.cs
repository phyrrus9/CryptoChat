using System;
using System.ComponentModel.DataAnnotations;

namespace ClientModels
{
    public class KeyExchange
    {
        public enum RequestType
        {
            SendKey, // send a PEM public key to a user
            RequestKey, // request a PEM public key from a user
        }
        [Key]
        public Guid Id { get; set; } // set by server
        [Required] public byte[] UserId { get; set; } //encrypted id of the user sending the request
        [Required] public RequestType Type { get; set; }
        [Required] public string KeyFingerprint { get; set; }
        public string KeyPEM { get; set; }
    }
}

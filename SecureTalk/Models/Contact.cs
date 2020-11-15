using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecureTalk.Models
{
    public class Contact
    {
        [Key, Required] public string UserId { get; set; } // phone number like
        [Required] public string ExchangeFingerprint { get; set; } // public key for exchange

        public string Name { get; set; }

        [JsonIgnore] public string DisplayName => Name ?? UserId;
        [JsonIgnore] public bool IsContact => !string.IsNullOrEmpty(Name);
    }
}

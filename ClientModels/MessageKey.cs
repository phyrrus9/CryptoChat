using System;
using System.ComponentModel.DataAnnotations;

namespace ClientModels
{
    public class MessageKey
    {
        [Key] public string Fingerprint { get; set; }
        [Required] public string PublicPem { get; set; }
        [Required] public DateTime Created = DateTime.UtcNow;
    }
}

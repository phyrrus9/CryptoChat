using System.ComponentModel.DataAnnotations;

namespace ClientModels.Users
{
    public class User
    {
        [Key] public string Id { get; set; }
        public string PreferredName { get; set; }
        [Required] public string ExchangePem { get; set; }
        [Required] public string ExchangeFingerprint { get; set; }
    }
}

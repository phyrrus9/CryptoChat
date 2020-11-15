using System.ComponentModel.DataAnnotations;

namespace ClientModels.Users
{
    public class CreateModel
    {
        [Required] public string ExchangePem { get; set; }
        public string DisplayName { get; set; }
    }
}

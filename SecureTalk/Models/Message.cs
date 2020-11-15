using System.Text.Json.Serialization;

namespace SecureTalk.Models
{
    public class Message : ClientModels.Message
    {
        [JsonIgnore] public string DecryptedSenderId { get; set; } // plaintext, local only
        [JsonIgnore] public string DecryptedRecipientId { get; set; } // plaintext, local only
    }
}

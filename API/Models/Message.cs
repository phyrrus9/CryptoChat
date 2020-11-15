using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Message : ClientModels.Message
    {
        [Required, JsonIgnore] public string DecryptedRecipientId { get; set; }
        [JsonIgnore] DateTime? Received { get; set; }
    }
}

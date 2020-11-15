using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SecureTalk.Models
{
    public class MessageKey : ClientModels.MessageKey
    {
        [JsonIgnore] public bool IsExchange { get; set; }
        [JsonIgnore] public string AssignedContactId { get; set; }
        [JsonIgnore] public string PrivatePem { get; set; }
        [JsonIgnore, ForeignKey("AssignedContactId")] public virtual Contact Contact { get; set; }
    }
}

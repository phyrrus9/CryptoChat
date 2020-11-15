using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class User : ClientModels.Users.User
    {
        [Required, JsonIgnore] public DateTime Created { get; set; }
        [JsonIgnore] public DateTime? Modified { get; set; }
        public void Merge(ClientModels.Users.EditModel mdl)
        {
            PreferredName = mdl.DisplayName ?? PreferredName;
            Modified = DateTime.UtcNow;
        }
    }
}

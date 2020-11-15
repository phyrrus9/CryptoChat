using Newtonsoft.Json;

namespace SecureTalk.Models
{
    public class User : ClientModels.Users.User
    {
        public static User FromBase(ref ClientModels.Users.User b) => JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(b));
        public bool Active { get; set; }
        public string PEMPath { get; set; }
    }
}

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIClient.v1
{
    public class Users : Core
    {
        public static async Task<ClientModels.Users.User> Register(ClientModels.Users.CreateModel mdl)
        {
            HttpResponseMessage response = await ExecuteAsync("/Users", Method.Post, mdl);
            return await DecodeAsync<ClientModels.Users.User>(response);
        }
        public static async Task<ClientModels.Users.User> Get(string Id)
        {
            HttpResponseMessage response = await ExecuteAsync($"/Users/{Id}", Method.Get);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await DecodeAsync<ClientModels.Users.User>(response),
                HttpStatusCode.NotFound => null,
                _ => throw new System.Exception(response.StatusCode.ToString())
            };
        }
        public static async Task<ClientModels.Users.User> Edit(string Id, ClientModels.Users.EditModel mdl)
        {
            HttpResponseMessage response = await ExecuteAsync($"/Users/{Id}", Method.Patch, mdl);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await DecodeAsync<ClientModels.Users.User>(response),
                HttpStatusCode.NotFound => null,
                _ => throw new System.Exception(response.StatusCode.ToString())
            };
        }
        public static async Task<ClientModels.Users.User> Me()
        {
            HttpResponseMessage response = await ExecuteAsync("/Users/Me", Method.Get, new ClientModels.SignedMessage());
            return await DecodeAsync<ClientModels.Users.User>(response);
        }
    }
}

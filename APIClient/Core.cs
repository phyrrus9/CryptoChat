using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APIClient
{
    public enum Method
    {
        Get,
        Post,
        Patch,
        Put,
        Delete
    }
    public static class Settings
    {
        private static AsymmetricKeyParameter _PublicKey, _PrivateKey;
        private static string _UserId;
        public static bool CacheKeys = true;
        public static Func<AsymmetricKeyParameter> GetPublicKey { set; private get; }
        public static Func<AsymmetricKeyParameter> GetPrivateKey { set; private get; }
        public static Func<string> GetUserId { set; internal get; }
        public static Func<bool> CheckConnection { set; internal get; }

        internal static AsymmetricKeyParameter PublicKey => CacheKeys ? _PublicKey ??= GetPublicKey.Invoke() : GetPublicKey.Invoke();
        internal static AsymmetricKeyParameter PrivateKey => CacheKeys ? _PrivateKey ??= GetPrivateKey.Invoke() : GetPrivateKey.Invoke();
        internal static string UserId => CacheKeys ? _UserId ??= GetUserId.Invoke() : GetUserId.Invoke();

        internal static void ClearCache()
        {
            _PublicKey = _PrivateKey = null;
            _UserId = null;
        }
    }
    public class Core
    {
        protected static HttpClient _client;
        static readonly HttpClientHandler clientHandler = new() { AutomaticDecompression = DecompressionMethods.None | DecompressionMethods.Deflate | DecompressionMethods.GZip };
        static readonly string Endpoint = "https://localhost:44397";

        protected static HttpClient Client()
        {
            if (_client == null)
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                _client = new(clientHandler) { BaseAddress = new Uri(Endpoint) };
            }
            return _client;
        }
        private static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 64, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;
            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            return httpContent;
        }
        public static HttpRequestMessage Request(string path, Method method, object body = null)
        {
            if (body is ClientModels.SignedMessage signed)
                signed.SignData(Settings.PublicKey);
            static string DecodeMethod(Method method) => method switch
            {
                Method.Get => "GET",
                Method.Patch => "PATCH",
                Method.Post => "POST",
                Method.Put => "PUT",
                Method.Delete => "DELETE",
                _ => "UNKNOWN"
            };
            Settings.CheckConnection?.Invoke();
            HttpRequestMessage request = new(new HttpMethod(DecodeMethod(method)), path)
            {
                Content = body == null ? null : CreateHttpContent(body)
            };
            return request;
        }
        public static HttpResponseMessage Execute(HttpRequestMessage request)
        {
            HttpClient client = Client();
            Task<HttpResponseMessage> responseTask = client.SendAsync(request);
            try { responseTask.Wait(); }
            catch (ObjectDisposedException)
            {
                _client = null;
                return Execute(request);
            }
            HttpResponseMessage response = responseTask.Result;
            return response;
        }
        public static async Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage request)
        {
            HttpClient client = Client();
            HttpResponseMessage response = null;
            try { response = await client.SendAsync(request); }
            catch (ObjectDisposedException)
            {
                _client = null;
                return await ExecuteAsync(request);
            }
            catch (Exception x)
            {
                var dbg = 5;
            }
            return response;
        }
        public static HttpResponseMessage Execute(string path, Method method, object body = null) => Execute(Request(path, method, body: body));
        public static async Task<HttpResponseMessage> ExecuteAsync(string path, Method method, object body = null) => await ExecuteAsync(Request(path, method, body: body));
        public static T Decode<T>(HttpResponseMessage message) => Task.Run(async () => await DecodeAsync<T>(message)).Result;
        public static async Task<T> DecodeAsync<T>(HttpResponseMessage message)
        {
            try { return JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync()); }
            catch (Exception)
            {
                return default;
            }
        }
    }
}

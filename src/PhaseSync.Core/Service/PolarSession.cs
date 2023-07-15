using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Service
{
    public sealed class PolarSession : ISession, IDisposable
    {
        private const string baseAddress = "https://flow.polar.com";
        private const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36";
        private readonly IScalar<HttpClient> client;

        public PolarSession(string username, string password)
        {
            this.client = new ScalarOf<HttpClient>(() =>
            {
                var client =
                    new HttpClient(
                        new HttpClientHandler()
                        { CookieContainer = new CookieContainer(), AllowAutoRedirect = true }
                    )
                    { BaseAddress = new Uri(baseAddress) };
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

                var loginData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", username),
                    new KeyValuePair<string, string>("password", password),
                });
                var responseMessage = client.PostAsync("/login", loginData).Result;

                //if (responseMessage.StatusCode != HttpStatusCode.OK || responseMessage.RequestMessage!.RequestUri!.AbsolutePath != "/diary")
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new Exception("Login to Polar Flow failed");
                }
                return client;
            });
        }

        public void Dispose()
        {
            this.client.Value().Dispose();
        }

        public JsonNode Get(string url)
        {
            var response = this.client.Value().GetAsync(url).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get request to polar flow ({url}) failed: {responseContent}");
            }
            return JsonNode.Parse(responseContent)!;
        }

        public JsonNode Post(string url, JsonNode content)
        {
            var stringContent = new StringContent(content.ToString(), Encoding.UTF8, "application/json");
            var response = this.client.Value().PostAsync(url, stringContent).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Post request to polar flow ({url}) failed: {responseContent}");
            }
            return JsonNode.Parse(responseContent)!;
        }
    }
}

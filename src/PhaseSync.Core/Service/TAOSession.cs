using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Service
{
    public sealed class TAOSession : ISession, IDisposable
    {
        private const string baseAddress = "https://beta.trainasone.com";
        private readonly IScalar<HttpClient> client;

        public TAOSession(string token)
        {
            this.client = new ScalarOf<HttpClient>(() =>
            {
                var client = new HttpClient() { BaseAddress = new Uri(baseAddress) };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                throw new Exception($"Get request to TrainAsONE ({url}) failed: {responseContent}");
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
                throw new Exception($"Post request to TrainAsONE ({url}) failed: {responseContent}");
            }
            return JsonNode.Parse(responseContent)!;
        }
    }
}

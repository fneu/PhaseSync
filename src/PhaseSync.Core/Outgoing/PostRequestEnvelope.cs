using System.Text;
using System.Text.Json.Nodes;

namespace PhaseSync.Core.Outgoing
{
    public abstract class PostRequestEnvelope : IRequest
    {
        private readonly string url;
        private readonly JsonNode content;

        public PostRequestEnvelope(string url, JsonNode content)
        {
            this.url = url;
            this.content = content;
        }
        public async Task<IResult> Send(HttpClient client)
        {
            var stringContent = new StringContent(content.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ResultOf(
                    true,
                    "",
                    JsonNode.Parse(responseContent)!
                );
            }

            return new ResultOf(
                false,
                responseContent,
                new JsonObject()
            );
        }
    }
}

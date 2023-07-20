using System.Text.Json.Nodes;

namespace PhaseSync.Core.Outgoing
{
    public abstract class GetRequestEnvelope : IRequest
    {
        private readonly string url;

        public GetRequestEnvelope(string url)
        {
            this.url = url;
        }
        public async Task<IResult> Send(HttpClient client)
        {
            var response = await client.GetAsync(url);
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

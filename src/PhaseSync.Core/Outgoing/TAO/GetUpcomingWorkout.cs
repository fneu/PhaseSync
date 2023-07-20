using System.Text.Json.Nodes;

namespace PhaseSync.Core.Outgoing.TAO
{
    public sealed class GetUpcomingWorkout : IRequest
    {
        private const string url = "/api/mobile/plannedWorkouts";

        public async Task<IResult> Send(HttpClient client)
        {
            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ResultOf(
                    true,
                    "",
                    JsonNode.Parse(responseContent)![0]!
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

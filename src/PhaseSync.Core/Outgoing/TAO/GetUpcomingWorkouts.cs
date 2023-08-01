using System.Text.Json.Nodes;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Outgoing.TAO
{
    public sealed class GetUpcomingWorkouts : IRequest
    {
        private const string url = "/api/mobile/plannedWorkouts";

        public async Task<IResult> Send(HttpClient client)
        {
            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var workouts = JsonNode.Parse(responseContent)!;
                return new ResultOf(
                    true,
                    "",
                    new JsonArray(
                        new Mapped<JsonNode, JsonNode>(
                            node => JsonNode.Parse(node.ToString())!,
                            new Filtered<JsonNode>(
                                workout => (string)workout["workoutVisibility"]! == "FULL",
                                JsonNode.Parse(responseContent)!.AsArray()!.ToArray()!
                            )
                        ).ToArray()
                    )
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

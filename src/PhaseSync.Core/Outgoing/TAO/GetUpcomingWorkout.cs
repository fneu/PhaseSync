using System.Text.Json.Nodes;
using Yaapii.Atoms.Collection;
using Yaapii.Atoms.Text;

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
                var workouts = JsonNode.Parse(responseContent)!;
                return new ResultOf(
                    true,
                    "",
                    new Filtered<JsonNode>(
                        workout => new BoolOf(workout["next"]!.ToString()).Value(),
                        JsonNode.Parse(responseContent)!.AsArray()!
                    ).First()
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

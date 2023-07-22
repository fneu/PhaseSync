using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace PhaseSync.Core.Outgoing.Polar
{
    public sealed partial class GetRunningProfile : IRequest
    {
        public async Task<IResult> Send(HttpClient client)
        {
            var response = await client.GetAsync("/settings/sports");
            var content = await response.Content.ReadAsStringAsync();

            var regex = DataProfileIdRegex();

            Match match = regex.Match(content);
            if (match.Success)
            {
                return new ResultOf(
                    true,
                    "",
                    JsonNode.Parse(match.Groups[1].Value)!
                );
            }

            return new ResultOf(
                false,
                "Running not found in the List of sport profiles in Polar Flow!",
                new JsonObject()
            );

        }

        [GeneratedRegex(@"data-profile-id=(\d+) data-sport-id=1")]
        private static partial Regex DataProfileIdRegex();
    }
}

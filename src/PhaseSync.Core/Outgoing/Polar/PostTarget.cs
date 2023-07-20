using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using System.Text;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Number;

namespace PhaseSync.Core.Outgoing.Polar
{
    public sealed class PostTarget : IRequest
    {
        private const string url = "/api/trainingtarget";
        private readonly IEntity<IHoneyComb> target;

        public PostTarget(IEntity<IHoneyComb> target)
        {
            this.target = target;
        }

        public async Task<IResult> Send(HttpClient client)
        {
            var json = new JsonObject()
            {
                ["type"] = "PHASED",
                ["name"] = new Title.Of(target).Value(),
                ["description"] = new Description.Of(target).Value(),
                ["datetime"] = new Time.Of(target).Value(),
                ["exerciseTargets"] = new JsonArray() {
                    new JsonObject()
                    {
                        ["id"] = null,
                        ["distance"] = null,
                        ["calories"] = null,
                        ["duration"] = null,
                        ["index"] = 0,
                        ["sportId"] = 1,
                        ["phases"] = new Phases.Of(target).Value()
                    }
                }
            };

            var stringContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                target.Update(new PolarID(new NumberOf(responseContent).AsInt()));
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

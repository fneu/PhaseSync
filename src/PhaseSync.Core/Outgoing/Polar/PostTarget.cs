using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using System.Text;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Number;

namespace PhaseSync.Core.Outgoing.Polar
{
    public sealed class PostTarget : IRequest
    {
        private const string url = "/api/trainingtarget";
        private readonly IEntity<IHoneyComb> target;
        private readonly IEntity<IProps> settings;

        public PostTarget(IEntity<IHoneyComb> target, IEntity<IProps> settings)
        {
            this.target = target;
            this.settings = settings;
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
                        ["phases"] = new JsonArray(
                            new Mapped<IEntity<IXocument>, JsonNode>(
                                phase => new PhaseAsPolarJson(phase, target.Memory(), settings).Value(),
                                new Phases.Of(target)
                                
                                ).ToArray()
                        )
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

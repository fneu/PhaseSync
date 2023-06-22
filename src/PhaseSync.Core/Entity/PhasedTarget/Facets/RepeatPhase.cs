using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Facets
{
    public sealed class RepeatPhase : ScalarEnvelope<JsonNode>
    {
        public RepeatPhase(JsonNode workoutStep, IEntity<IProps> settings) : base(() =>
            new JsonObject
            {
                ["phaseType"] = "REPEAT",
                ["repeatCount"] = (int)workoutStep["multiple"]!,
                ["phases"] = new JsonArray(
                    new Mapped<JsonNode, JsonNode>(
                        json => new Phase(json, settings).Value(),
                        workoutStep["subSteps"]!.AsArray()!
                    ).ToArray()
                )
            }
        )
        { }
    }
}

using PhaseSync.Core.Units;
using System.Text.Json.Nodes;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Facets
{
    public sealed class OpenDurationPhase : ScalarEnvelope<JsonNode>
    {
        public OpenDurationPhase(JsonNode workoutStep) : base(() =>
            new JsonObject
            {
                ["id"] = null,
                ["lowerZone"] = null,
                ["upperZone"] = null,
                ["intensityType"] = null,
                ["phaseChangeType"] = "AUTOMATIC",
                ["goalType"] = "DURATION",
                ["duration"] = new PolarDuration((int)workoutStep["duration"]!).AsString(),
                ["distance"] = null,
                ["name"] = (string)workoutStep["workoutStepType"]!,
                ["phaseType"] = "PHASE"
            }
        )
        { }
    }
}

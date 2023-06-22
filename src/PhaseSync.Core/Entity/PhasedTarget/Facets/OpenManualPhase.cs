using System.Text.Json.Nodes;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Facets
{
    public sealed class OpenManualPhase : ScalarEnvelope<JsonNode>
    {
        public OpenManualPhase(JsonNode workoutStep) : base(() =>
            new JsonObject
            {
                ["id"] = null,
                ["lowerZone"] = null,
                ["upperZone"] = null,
                ["intensityType"] = null,
                ["phaseChangeType"] = "MANUAL",
                ["goalType"] = "DURATION",
                ["duration"] = "00:01:00",
                ["distance"] = null,
                ["name"] = (string)workoutStep["workoutStepType"]!,
                ["phaseType"] = "PHASE"
            }
        )
        { }
    }
}

using System.Text.Json.Nodes;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Facets
{
    public sealed class OpenDistancePhase : ScalarEnvelope<JsonNode>
    {
        public OpenDistancePhase(JsonNode workoutStep) : base(() =>
            new JsonObject
            {
                ["id"] = null,
                ["lowerZone"] = null,
                ["upperZone"] = null,
                ["intensityType"] = null,
                ["phaseChangeType"] = "AUTOMATIC",
                ["goalType"] = "DISTANCE",
                ["duration"] = "00:00:00",
                ["distance"] = (double)workoutStep["distance"]!,
                ["name"] = (string)workoutStep["workoutStepType"]!,
                ["phaseType"] = "PHASE"
            }
        )
        { }
    }
}

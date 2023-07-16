using PhaseSync.Core.Units;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Facets
{
    public sealed class SpeedDurationPhase : ScalarEnvelope<JsonNode>
    {
        public SpeedDurationPhase(JsonNode workoutStep, IEntity<IProps> settings) : base(() =>
            new JsonObject
            {
                ["id"] = null,
                ["lowerZone"] = 1,
                ["upperZone"] = 5,
                ["intensityType"] = "SPEED_ZONES",
                ["phaseChangeType"] = "AUTOMATIC",
                ["goalType"] = "DURATION",
                ["duration"] = new PolarDuration((int)workoutStep["duration"]!).AsString(),
                ["distance"] = null,
                ["name"] = new Pace((double)workoutStep["velocity"]!, settings).AsString(),
                ["phaseType"] = "PHASE"
            }
        )
        { }
    }
}

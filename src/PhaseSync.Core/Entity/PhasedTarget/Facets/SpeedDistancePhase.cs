using PhaseSync.Core.Units;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Facets
{
    public sealed class SpeedDistancePhase : ScalarEnvelope<JsonNode>
    {
        public SpeedDistancePhase(JsonNode workoutStep, IEntity<IProps> settings) : base(() =>
            new JsonObject
            {
                ["id"] = null,
                ["lowerZone"] = 1, // TODO: PHASE
                ["upperZone"] = 5, // TODO: PHASE
                ["intensityType"] = "SPEED_ZONES",
                ["phaseChangeType"] = "AUTOMATIC",
                ["goalType"] = "DISTANCE",
                ["duration"] = "00:00:00",
                ["distance"] = (double)workoutStep["distance"]!,
                ["name"] = new Pace((double)workoutStep["velocity"]!, settings).AsString(),
                ["phaseType"] = "PHASE"
            }
        )
        { }
    }
}

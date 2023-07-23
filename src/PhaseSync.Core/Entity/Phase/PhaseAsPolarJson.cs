using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Units;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class PhaseAsPolarJson : ScalarEnvelope<JsonNode>
    {
        public PhaseAsPolarJson(string id, IHoneyComb comb, IHive hive) : this(
            new PhaseOf(comb, id),
            comb,
            new SettingsOf(hive)
        )
        { }

        public PhaseAsPolarJson(IEntity<IXocument> phase, IHoneyComb comb, IEntity<IProps> settings) : base(
            () =>
            {
                if (new SubPhases.Has(phase).Value())
                {
                    return new JsonObject
                    {
                        ["phaseType"] = "REPEAT",
                        ["repeatCount"] = new SubPhases.RepeatCount(phase).Value(),
                        ["phases"] = new JsonArray(
                            new Mapped<string, JsonNode>(
                                id => new PhaseAsPolarJson(new PhaseOf(comb, id), comb, settings).Value(),
                                new SubPhases.IDs(phase)
                            ).ToArray()
                        )
                    };
                }

                return new JsonObject
                {
                    ["id"] = null,
                    ["lowerZone"] = new SpeedGoal.Has(phase).Value() ? new SpeedGoal.LowerZone(phase, settings).Value() : null,
                    ["upperZone"] = new SpeedGoal.Has(phase).Value() ? new SpeedGoal.UpperZone(phase, settings).Value() : null,
                    ["intensityType"] = new SpeedGoal.Has(phase).Value() ? "SPEED_ZONES" : null,
                    ["phaseChangeType"] = new PhaseChangeType.Of(phase).Value(),
                    ["goalType"] = new DistanceGoal.Has(phase).Value() ? "DISTANCE" : "DURATION",
                    ["duration"] = new DistanceGoal.Has(phase).Value() ? "00:00:00" : new PolarDuration(new DurationGoal.InSeconds(phase).Value()).AsString(),
                    ["distance"] = new DistanceGoal.Has(phase).Value() ? new DistanceGoal.InMeters(phase).Value() : null,
                    ["name"] = new Name.Of(phase).AsString(),
                    ["phaseType"] = "PHASE"
                };
            }
        )
        { }
    }
}

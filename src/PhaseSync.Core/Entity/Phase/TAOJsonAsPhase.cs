using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class TAOJsonAsPhase : ScalarEnvelope<IEntity<IXocument>>
    {
        public TAOJsonAsPhase(JsonNode workoutStep, IHoneyComb comb, IEntity<IProps> settings) : base(
            () =>
            {
                if (workoutStep["subSteps"] is not null)
                {
                    return new RepeatPhase(workoutStep, comb, settings);
                }

                return new FallbackMap<string, IEntity<IXocument>>(
                    new MapOf<string, IEntity<IXocument>>(
                        new KvpOf<string, IEntity<IXocument>>("BRISK_WALK", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("COOLDOWN", () => new OpenManualPhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("CUSTOM", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("EASY", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("EXTREME_DISTANCE", () => new OpenDistancePhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("EXTREME_DURATION", () => new OpenDurationPhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("FAST", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("INTERVAL_FAST", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("PERCEIVED_CONVERSATIONAL", () => new OpenDurationPhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("PERCEIVED_NATURAL", () => new OpenDurationPhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("PERCEIVED_WARMUP", () => new OpenDurationPhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("PICKUP_FAST", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("PREPARATION", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("RECOVERY", () => new OpenDurationPhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("REPETITION_FAST", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("STANDING", () => new OpenDurationPhase(workoutStep, comb)),
                        new KvpOf<string, IEntity<IXocument>>("TABATA_FAST", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("THRESHOLD_FAST", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("VERY_EASY", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("WALK", () => new SpeedDurationPhase(workoutStep, comb, settings)),
                        new KvpOf<string, IEntity<IXocument>>("WARMUP", () => new SpeedDurationPhase(workoutStep, comb, settings))
                    ),
                    unknown => new SpeedDurationPhase(workoutStep, comb, settings)
                )[(string)workoutStep["workoutStepType"]!];
            }
        )
        { }
    }
}

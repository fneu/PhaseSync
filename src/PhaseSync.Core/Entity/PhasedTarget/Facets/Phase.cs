using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Facets
{
    public sealed class Phase : ScalarEnvelope<JsonNode>
    {
        public Phase(JsonNode workoutStep, IEntity<IProps> settings) : base(() =>
            new FallbackMap<int, IScalar<JsonNode>>(
                new MapOf<int, IScalar<JsonNode>>(
                    new KvpOf<int, IScalar<JsonNode>>(1,
                        () => new FallbackMap<string, IScalar<JsonNode>>(
                            new MapOf<string, IScalar<JsonNode>>(
                                new KvpOf<string, IScalar<JsonNode>>("BRISK_WALK", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("COOLDOWN", () => new OpenManualPhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("CUSTOM", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("EASY", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("EXTREME_DISTANCE", () => new OpenDistancePhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("EXTREME_DURATION", () => new OpenDurationPhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("FAST", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("INTERVAL_FAST", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("PERCEIVED_CONVERSATIONAL", () => new OpenDurationPhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("PERCEIVED_NATURAL", () => new OpenDurationPhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("PERCEIVED_WARMUP", () => new OpenDurationPhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("PICKUP_FAST", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("PREPARATION", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("RECOVERY", () => new OpenDurationPhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("REPETITION_FAST", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("STANDING", () => new OpenDurationPhase(workoutStep)),
                                new KvpOf<string, IScalar<JsonNode>>("TABATA_FAST", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("THRESHOLD_FAST", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("VERY_EASY", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("WALK", () => new SpeedDurationPhase(workoutStep, settings)),
                                new KvpOf<string, IScalar<JsonNode>>("WARMUP", () => new SpeedDurationPhase(workoutStep, settings))
                            ),
                            unknown => new SpeedDurationPhase(workoutStep, settings)
                        )[(string)workoutStep["workoutStepType"]!]
                    )
                ),
                more => new RepeatPhase(workoutStep, settings)
            )[(int)workoutStep["multiple"]!].Value()
        )
        { }
    }
}

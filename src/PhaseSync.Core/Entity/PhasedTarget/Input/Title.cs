using Xive;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class Title : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "title";

        public Title(string activitySubType) : base(
            (comb) => comb.Props().Refined(KEY, NiceName(activitySubType))

            )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IHoneyComb> target) : base(
               () => target.Memory().Props().Names().Contains(KEY)
            )
            { }
        }

        public sealed class Of : ScalarEnvelope<string>
        {
            public Of(IEntity<IHoneyComb> target) : base(
                () => target.Memory().Props().Value(KEY)
            )
            { }
        }

        private static string NiceName(string activitySubType)
        {
            return new FallbackMap(
                new MapOf(
                    new KvpOf("ASSESSMENT_3200_METRE", "3.2 km Assessment"),
                    new KvpOf("ASSESSMENT_6_MIN", "6 min Assessment"),
                    new KvpOf("ASSESSMENT_PERCEIVED_EFFORT", "Perceived Effort Run"),
                    new KvpOf("PARKRUN", "Parkrun"),
                    new KvpOf("RACE_ROAD_BEST_EFFORT", "Best Effort Road Race"),
                    new KvpOf("RACE_ROAD_CASUAL", "Casual Road Race"),
                    new KvpOf("RACE_TRAIL_BEST_EFFORT", "Best Effort Trail Race"),
                    new KvpOf("RACE_TRAIL_CASUAL", "Casual Trail Race"),
                    new KvpOf("REST", "Rest"),
                    new KvpOf("RUN_WALK", "Run/Walk"),
                    new KvpOf("TRAINING_ECONOMY", "Economy Run"),
                    new KvpOf("TRAINING_ECONOMY_PLUS", "Economy+ Run"),
                    new KvpOf("TRAINING_INTERVAL", "Interval Run"),
                    new KvpOf("TRAINING_PICKUP", "Pickup Run"),
                    new KvpOf("TRAINING_PROGRESSION", "Progression Run"),
                    new KvpOf("TRAINING_RECOVERY", "Recovery Run"),
                    new KvpOf("TRAINING_REPETITION", "Repetition Run"),
                    new KvpOf("TRAINING_RUN_WALK", "Run/Walk"),
                    new KvpOf("TRAINING_TABATA", "Tabata Run"),
                    new KvpOf("TRAINING_THRESHOLD", "Threshold Run")
                ),
                unknown => "Run"
            )[activitySubType];

        }
    }
}

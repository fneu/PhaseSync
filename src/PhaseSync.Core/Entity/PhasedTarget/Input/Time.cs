using PhaseSync.Core.Units;
using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class Time : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "timestamp";

        public Time(string utcTime, string localTimeZone) : base(
            (comb) => comb.Props().Refined(KEY, new LocalTime(utcTime, localTimeZone).Value())

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
    }
}

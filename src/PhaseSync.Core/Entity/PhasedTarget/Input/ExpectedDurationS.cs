using Xive;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class ExpectedDurationS : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "expected.time";

        public ExpectedDurationS(int seconds) : base(
            (comb) => comb.Props().Refined(KEY, new TextOf(seconds).AsString())

            )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IHoneyComb> target) : base(
               () => target.Memory().Props().Names().Contains(KEY)
            )
            { }
        }

        public sealed class Of : ScalarEnvelope<int>
        {
            public Of(IEntity<IHoneyComb> target) : base(
                () => new NumberOf(target.Memory().Props().Value(KEY)).AsInt()
            )
            { }
        }
    }
}

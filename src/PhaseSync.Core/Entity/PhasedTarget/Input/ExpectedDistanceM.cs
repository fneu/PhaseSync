using Xive;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class ExpectedDistanceM : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "expected.dist";

        public ExpectedDistanceM(double dist) : base(
            (comb) => comb.Props().Refined(KEY, new TextOf(dist).AsString())

            )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IHoneyComb> target) : base(
               () => target.Memory().Props().Names().Contains(KEY)
            )
            { }
        }

        public sealed class Of : ScalarEnvelope<double>
        {
            public Of(IEntity<IHoneyComb> target) : base(
                () => new NumberOf(target.Memory().Props().Value(KEY)).AsDouble()
            )
            { }
        }
    }
}

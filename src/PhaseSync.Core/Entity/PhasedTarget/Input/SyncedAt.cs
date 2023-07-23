using Xive;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class SyncedAt : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "synced.at";

        public SyncedAt(string timestamp) : base(
            (comb) => comb.Props().Refined(KEY, timestamp)

            )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IHoneyComb> target) : base(
               () => target.Memory().Props().Names().Contains(KEY)
            )
            { }
        }

        public sealed class Of : TextEnvelope
        {
            public Of(IEntity<IHoneyComb> target) : base(
                () => target.Memory().Props().Value(KEY),
                false
            )
            { }
        }
    }
}

using PhaseSync.Core.Entity.Phase;
using Xive;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{

    public sealed class Phases : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "phases";

        public Phases(IEnumerable<IEntity<IXocument>> phases) : base(
            (comb) => comb.Props().Refined(
                KEY,
                new Mapped<IEntity<IXocument>, string>(
                    phase => phase.ID(),
                    phases
                ).ToArray()
            )
        )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IHoneyComb> target) : base(
               () => target.Memory().Props().Names().Contains(KEY)
            )
            { }
        }

        public sealed class Of : ListEnvelope<IEntity<IXocument>>
        {
            public Of(IEntity<IHoneyComb> target) : base(
                () => new Mapped<string, IEntity<IXocument>>(
                    id => new PhaseOf(target.Memory(), id),
                    target.Memory().Props().Values(KEY)),
                false
            )
            { }
        }
    }
}

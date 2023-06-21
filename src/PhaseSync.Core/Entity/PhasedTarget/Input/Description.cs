using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class Description : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "description";

        public Description(IEnumerable<string> lines) : this(lines.ToArray())
        { }

        public Description(params string[] lines) : base(
            comb => comb.Props().Refined(KEY, lines)
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
                () => string.Join("\n", target.Memory().Props().Values(KEY))
            )
            { }
        }
    }
}

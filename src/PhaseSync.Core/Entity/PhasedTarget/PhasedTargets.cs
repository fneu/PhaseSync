using Xive;
using Yaapii.Atoms.Enumerable;

namespace PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class PhasedTargets : ManyEnvelope<IEntity<IHoneyComb>>
    {
        public PhasedTargets(IHive userHive) : base(() =>
            new Mapped<IHoneyComb, IEntity<IHoneyComb>>(
                comb => new PhasedTargetOf(() => comb, comb.Name().Substring(userHive.Scope().Length + 1)),
                new Filtered<IHoneyComb>(
                        comb => !comb.Name().EndsWith("settings"),
                        userHive.Catalog().List()
                    )
            ),
            false
        )
        { }
    }
}

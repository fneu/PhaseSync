using Xive;
using Yaapii.Atoms.Enumerable;

namespace PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class PhasedTargetCollection : ManyEnvelope<IEntity<IHoneyComb>>
    {
        public PhasedTargetCollection(IHive userHive) : base(() =>
            new Mapped<IHoneyComb, IEntity<IHoneyComb>>(
                comb => new PhasedTargetOf(userHive, comb.Name().Substring(userHive.Scope().Length + 1)),
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

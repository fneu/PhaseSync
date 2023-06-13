using Xive;

namespace PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class PhasedTargetOf : EntityEnvelope<IHoneyComb>
    {
        public PhasedTargetOf(IHive UserHive, string id) : base(
            new EntityOf<IHoneyComb>(
                id,
                UserHive.Comb(id)
                )
            )
        { }
    }
}

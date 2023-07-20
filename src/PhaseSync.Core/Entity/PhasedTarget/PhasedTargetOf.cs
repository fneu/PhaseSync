using Xive;

namespace PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class PhasedTargetOf : EntityEnvelope<IHoneyComb>
    {
        public PhasedTargetOf(IHive UserHive, string id) : this(() => UserHive.Comb(id), id)
        { }

        public PhasedTargetOf(Func<IHoneyComb> comb, string id) : base(new EntityOf<IHoneyComb>(id, comb))
        { }
    }
}

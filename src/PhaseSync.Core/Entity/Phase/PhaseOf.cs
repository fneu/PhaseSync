using Xive;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class PhaseOf : EntityEnvelope<IXocument>
    {
        public PhaseOf(IHoneyComb comb, string id) : this(() => comb.Xocument(id), id)
        { }

        private PhaseOf(Func<IXocument> xocument, string id) : base(new EntityOf<IXocument>(id, xocument))
        { }
    }
}

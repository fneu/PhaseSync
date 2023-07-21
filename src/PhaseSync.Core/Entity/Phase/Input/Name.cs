using Xive;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace PhaseSync.Core.Entity.Phase.Input
{
    public sealed class Name : EntityInputEnvelope<IXocument>
    {
        public Name(string name) : base(
            xocument => xocument.Modify(
                new Directives()
                    .Xpath("/*")
                    .Add("name")
                    .Set(name)
            )
        )
        { }

        public sealed class Of : TextEnvelope
        {
            public Of(IEntity<IXocument> phase) : base(() =>
                phase.Memory().Value("/*/name/text()", ""),
                false
            )
            { }
        }
    }
}

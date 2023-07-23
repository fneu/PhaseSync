using Xive;
using Yaapii.Atoms.Scalar;
using Yaapii.Xambly;

namespace PhaseSync.Core.Entity.Phase.Input
{
    public sealed class PhaseChangeType : EntityInputEnvelope<IXocument>
    {
        public PhaseChangeType(string type) : base(
            xocument => xocument.Modify(
                new Directives()
                    .Xpath("/*")
                    .Add("phase-change-type")
                    .Set(type)
            )
        )
        { }

        public sealed class Of : ScalarEnvelope<string>
        {
            public Of(IEntity<IXocument> phase) : base(() =>
                new Valid(phase.Memory().Value("/*/phase-change-type/text()", "")).Value()
            )
            { }
        }

        private sealed class Valid : ScalarEnvelope<string>
        {
            public Valid(string value) : base(
                () => new string[] { "MANUAL", "AUTOMATIC" }.Contains(value) ? value : "AUTOMATIC"
            )
            { }
        }
    }
}

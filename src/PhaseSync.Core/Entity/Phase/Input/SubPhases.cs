using Xive;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace PhaseSync.Core.Entity.Phase.Input
{
    public sealed class SubPhases : EntityInputEnvelope<IXocument>
    {
        public SubPhases(int repeatCount, params IEntity<IXocument>[] phases) : base(
            xocument =>
            {
                var patch = new Directives()
                    .Xpath("/*")
                    .Add("sub-phases")
                    .Add("repeat-count")
                    .Set(new TextOf(repeatCount).AsString())
                    .Up()
                    .Add("phases");

                foreach (var phase in phases)
                {
                    patch.Push()
                        .Add("phase")
                        .Set(phase.ID())
                        .Up();
                }

                xocument.Modify(patch);
            }
        )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IXocument> phase) : base(() =>
                phase.Memory().Value("/*/sub-phases/repeat-count/text()", "") != ""
            )
            { }
        }

        public sealed class RepeatCount : ScalarEnvelope<int>
        {
            public RepeatCount(IEntity<IXocument> phase) : base(() =>
                new NumberOf(phase.Memory().Value("/*/sub-phases/repeat-count/text()", "")).AsInt()
            )
            { }
        }

        public sealed class IDs : ListEnvelope<string>
        {
            public IDs(IEntity<IXocument> phase) : base(() =>
                phase.Memory().Values("/*/sub-phases/phases/phase/text()"),
                false
            )
            { }
        }
    }
}

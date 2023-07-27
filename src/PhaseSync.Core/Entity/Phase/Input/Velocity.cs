using Xive;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace PhaseSync.Core.Entity.Phase.Input
{
    public sealed class Velocity : EntityInputEnvelope<IXocument>
    {
        public Velocity(double mps) : base(
            xocument => xocument.Modify(
                new Directives()
                    .Xpath("/*")
                    .Add("velocity")
                    .Set(new TextOf(mps).AsString())
            )
        )
        { }

        public sealed class InMPS : ScalarEnvelope<double>
        {
            public InMPS(IEntity<IXocument> phase) : base(() =>
                new NumberOf(phase.Memory().Value("/*/velocity/text()", "1.0")).AsDouble()
            )
            { }
        }
    }
}

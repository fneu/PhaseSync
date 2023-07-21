using PhaseSync.Core.Units;
using Xive;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace PhaseSync.Core.Entity.Phase.Input
{
    public sealed class DistanceGoal : EntityInputEnvelope<IXocument>
    {
        public DistanceGoal(double meters) : base(
            xocument => xocument.Modify(
                new Directives()
                    .Xpath("/*")
                    .Add("distance-goal")
                    .Set(new TextOf(meters).AsString())
            )
        )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IXocument> phase) : base(() =>
                phase.Memory().Value("/*/distance-goal/text()", "") != ""
            )
            { }
        }

        public sealed class InMeters : ScalarEnvelope<double>
        {
            public InMeters(IEntity<IXocument> phase) : base(() =>
                new NumberOf(phase.Memory().Value("/*/distance-goal/text()", "")).AsDouble()
            )
            { }
        }
    }
}

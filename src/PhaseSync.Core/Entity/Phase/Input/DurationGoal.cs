using Xive;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace PhaseSync.Core.Entity.Phase.Input
{
    public sealed class DurationGoal : EntityInputEnvelope<IXocument>
    {
        public DurationGoal(int seconds) : base(
            xocument => xocument.Modify(
                new Directives()
                    .Xpath("/*")
                    .Add("duration-goal")
                    .Set(new TextOf(seconds).AsString())
            )
        )
        { }

        public sealed class InSeconds : ScalarEnvelope<int>
        {
            public InSeconds(IEntity<IXocument> phase) : base(() =>
                new NumberOf(phase.Memory().Value("/*/duration-goal/text()", "60")).AsInt()
            )
            { }
        }
    }
}

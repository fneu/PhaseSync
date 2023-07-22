using PhaseSync.Core.Entity.Settings.Input;
using Xive;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;
using Yaapii.Xambly;

namespace PhaseSync.Core.Entity.Phase.Input
{
    public sealed class SpeedGoal : EntityInputEnvelope<IXocument>
    {
        public SpeedGoal(double mps) : base(
            xocument => xocument.Modify(
                new Directives()
                    .Xpath("/*")
                    .Add("speed-goal")
                    .Set(new TextOf(mps).AsString())
            )
        )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IXocument> phase) : base(() =>
                phase.Memory().Value("/*/speed-goal/text()", "") != ""
            )
            { }
        }

        public sealed class InMPS : ScalarEnvelope<double>
        {
            public InMPS(IEntity<IXocument> phase) : base(() =>
                new NumberOf(phase.Memory().Value("/*/speed-goal/text()", "")).AsDouble()
            )
            { }
        }

        public sealed class LowerZone : ScalarEnvelope<int>
        {
            public LowerZone(IEntity<IXocument> phase, IEntity<IProps> settings) : base(
                () =>
                {
                    if (!new SetZones.Has(settings).Value()
                        || !new SetZones.Of(settings).Value()
                        || !new ZoneLowerBounds.Has(settings).Value()
                        || !new ZoneRadius.Has(settings).Value())
                    {
                        return 1;
                    }
                    var targetSpeed = new InMPS(phase).Value();
                    var zoneLowerBounds = new ZoneLowerBounds.Of(settings);
                    var radius = new ZoneRadius.Of(settings).Value();

                    for (var i = 0; i < 5; i++)
                    {
                        if (targetSpeed - radius < zoneLowerBounds[i])
                        {
                            return new Max<int>(i, 1).Value();
                        }
                    }
                    return 5;
                }
            )
            { }
        }

        public sealed class UpperZone : ScalarEnvelope<int>
        {
            public UpperZone(IEntity<IXocument> phase, IEntity<IProps> settings) : base(
                () =>
                {
                    if (!new SetZones.Has(settings).Value()
                        || !new SetZones.Of(settings).Value()
                        || !new ZoneLowerBounds.Has(settings).Value()
                        || !new ZoneRadius.Has(settings).Value())
                    {
                        return 5;
                    }
                    var targetSpeed = new InMPS(phase).Value();
                    var zoneLowerBounds = new ZoneLowerBounds.Of(settings);
                    var radius = new ZoneRadius.Of(settings).Value();

                    for (var i = 0; i < 5; i++)
                    {
                        if (targetSpeed + radius <= zoneLowerBounds[i])
                        {
                            return new Max<int>(i, 1).Value();
                        }
                    }
                    return 5;
                }
            )
            { }
        }
    }
}

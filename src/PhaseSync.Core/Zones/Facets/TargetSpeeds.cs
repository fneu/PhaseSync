using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using Xive;
using Yaapii.Atoms.Enumerable;

namespace PhaseSync.Core.Zones.Facets
{
    public sealed class TargetSpeeds : ManyEnvelope<double>
    {
        public TargetSpeeds(IEntity<IHoneyComb> target) : base(
            () =>
            {
                var speeds = new List<double>();
                foreach (var phase in new Phases.Of(target))
                {
                    if (new SubPhases.Has(phase).Value())
                    {
                        foreach (var id in new SubPhases.IDs(phase))
                        {
                            var subphase = new PhaseOf(target.Memory(), id);
                            if (new SpeedGoal.Has(subphase).Value())
                            {
                                speeds.Add(new SpeedGoal.InMPS(subphase).Value());
                            }
                        }
                    }
                    else
                    {
                        if (new SpeedGoal.Has(phase).Value())
                        {
                            speeds.Add(new SpeedGoal.InMPS(phase).Value());
                        }
                    }
                }
                return new Distinct<double>(
                    new Sorted<double>(
                        speeds
                    )
                );
            },
            false)
        { }
    }
}

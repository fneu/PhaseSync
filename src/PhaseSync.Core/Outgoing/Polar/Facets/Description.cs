using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Units;
using Xive;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Outgoing.Polar.Facets
{
    public sealed class Description : TextEnvelope
    {
        public Description(IEntity<IHoneyComb> target, IEntity<IProps> settings) : base(
            () =>
            {
                var lines = new List<string>()
                {
                    new HumanReadableDuration(new ExpectedDurationS.Of(target).Value()).AsString(),
                    new HumanReadableDistance(new ExpectedDistanceM.Of(target).Value(), settings).AsString(),
                    "synced " + new SyncedAt.Of(target).AsString(),
                    "",
                    "--- Phases ---"
                };

                foreach (var phase in new Phases.Of(target))
                {
                    if (new SubPhases.Has(phase).Value())
                    {
                        lines.Add($"repeat {new SubPhases.RepeatCount(phase).Value()} x (");
                        foreach (var id in new SubPhases.IDs(phase))
                        {
                            lines.Add(Phasetitle(new PhaseOf(target.Memory(), id), settings));
                        }
                        lines.Add(")");
                    }
                    else
                    {
                        lines.Add(Phasetitle(phase, settings));
                    }
                }
                return string.Join("\n", lines);
            },
            false
        )
        { }

        private static string Phasetitle(IEntity<IXocument> phase, IEntity<IProps> settings)
        {
            if (new DistanceGoal.Has(phase).Value())
            {
                return $"{new HumanReadableDistance(new DistanceGoal.InMeters(phase).Value(), settings).AsString()} @ {new Name.Of(phase).AsString()}";
            }
            else
            {
                return $"{new HumanReadableDuration(new Duration.InSeconds(phase).Value()).AsString()} @ {new Name.Of(phase).AsString()}";
            }
        }
    }
}

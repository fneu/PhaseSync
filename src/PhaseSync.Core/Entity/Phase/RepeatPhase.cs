using PhaseSync.Core.Entity.Phase.Input;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Enumerable;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class RepeatPhase : EntityEnvelope<IXocument>
    {
        public RepeatPhase(JsonNode workoutStep, IHoneyComb comb, IEntity<IProps> settings) : base(
            () =>
            {
                var phase = new PhaseOf(comb);
                phase.Update(
                    new SubPhases(
                        (int)workoutStep["multiple"]!,
                        new Mapped<JsonNode, IEntity<IXocument>>(
                            step => new TAOJsonAsPhase(step, comb, settings).Value(),
                            workoutStep["subSteps"]!.AsArray()!
                        ).ToArray()
                    )
                );
                return phase;
            }
        )
        { }
    }
}

using PhaseSync.Core.Entity.Phase.Input;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class OpenDurationPhase : EntityEnvelope<IXocument>
    {
        public OpenDurationPhase(JsonNode workoutStep, IHoneyComb comb) : base(
            () =>
            {
                var phase = new PhaseOf(comb);
                phase.Update(
                    new Duration((int)workoutStep["duration"]!),
                    new Velocity((double)workoutStep["velocity"]!),
                    new Name((string)workoutStep["workoutStepType"]!)
                    );
                return phase;
            }
        )
        { }
    }
}

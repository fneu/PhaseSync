using PhaseSync.Core.Entity.Phase.Input;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class OpenDistancePhase : EntityEnvelope<IXocument>
    {
        public OpenDistancePhase(JsonNode workoutStep, IHoneyComb comb) : base(
            () =>
            {
                var phase = new PhaseOf(comb);
                phase.Update(
                    new DistanceGoal((double)workoutStep["distance"]!),
                    new Name((string)workoutStep["workoutStepType"]!)
                    );
                return phase;
            }
        )
        { }
    }
}

using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Units;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Core.Entity.Phase
{
    public sealed class SpeedDurationPhase : EntityEnvelope<IXocument>
    {
        public SpeedDurationPhase(JsonNode workoutStep, IHoneyComb comb, IEntity<IProps> settings) : base(
            () =>
            {
                var phase = new PhaseOf(comb);
                phase.Update(
                    new SpeedGoal((double)workoutStep["velocity"]!),
                    new Duration((int)workoutStep["duration"]!),
                    new Velocity((double)workoutStep["velocity"]!),
                    new Name(new Pace((double)workoutStep["velocity"]!, settings).AsString())
                    );
                return phase;
            }
        )
        { }
    }
}

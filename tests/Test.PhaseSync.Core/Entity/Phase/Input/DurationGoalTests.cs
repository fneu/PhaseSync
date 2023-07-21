using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Phase.Input
{
    public sealed class DurationGoalTests
    {
        [Fact]
        public void RetrievesDurationGoal()
        {
            var phase = new PhaseOf(new RamHive("test_user_id").Comb("test_program_id"), "test_phase_id");
            phase.Update(new DurationGoal(300));
            Assert.Equal(300, new DurationGoal.InSeconds(phase).Value());
        }

        [Fact]
        public void FallbackDurationGoal()
        {
            var phase = new PhaseOf(new RamHive("test_user_id").Comb("test_program_id"), "test_phase_id");
            Assert.Equal(60, new DurationGoal.InSeconds(phase).Value());
        }
    }
}

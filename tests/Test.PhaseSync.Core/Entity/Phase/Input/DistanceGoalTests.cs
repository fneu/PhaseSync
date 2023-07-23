using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Phase.Input
{
    public sealed class DistanceGoalTests
    {
        [Fact]
        public void SetsDistanceGoal()
        {
            var phase = new PhaseOf(new RamHive("test_user_id").Comb("test_program_id"), "test_phase_id");
            phase.Update(new DistanceGoal(5000));
            Assert.True(new DistanceGoal.Has(phase).Value());
        }

        [Fact]
        public void RetrievesDistanceGoal()
        {
            var phase = new PhaseOf(new RamHive("test_user_id").Comb("test_program_id"), "test_phase_id");
            phase.Update(new DistanceGoal(5000));
            Assert.Equal(5000.0, new DistanceGoal.InMeters(phase).Value());
        }
    }
}

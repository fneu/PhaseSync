using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Phase.Input
{
    public sealed class PhaseChangeTypeTests
    {
        [Fact]
        public void SetsPhaseChangeType()
        {
            var phase = new PhaseOf(new RamHive("test_user_id").Comb("test_program_id"), "test_phase_id");
            phase.Update(new PhaseChangeType("MANUAL"));
            Assert.Equal("MANUAL", new PhaseChangeType.Of(phase).Value());
        }
        [Fact]

        public void DefaultsToAutomatic()
        {
            var phase = new PhaseOf(new RamHive("test_user_id").Comb("test_program_id"), "test_phase_id");
            Assert.Equal("AUTOMATIC", new PhaseChangeType.Of(phase).Value());
        }
    }
}

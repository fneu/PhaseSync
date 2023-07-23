using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Phase.Input
{
    public sealed class NameTests
    {
        [Fact]
        public void RetrievesName()
        {
            var phase = new PhaseOf(new RamHive("test_user_id").Comb("test_program_id"), "test_phase_id");
            phase.Update(new Name("EXTREME_DISTANCE"));
            Assert.Equal("EXTREME_DISTANCE", new Name.Of(phase).AsString());
        }
    }
}

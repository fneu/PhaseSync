using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Phase.Input
{
    public sealed class SubPhasesTests
    {
        [Fact]
        public void SetsSubPhases()
        {
            var hive = new RamHive("test_user_id");
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SubPhases(2));
            Assert.True(new SubPhases.Has(phase).Value());
        }

        [Fact]
        public void RetrievesRepeatCount()
        {
            var hive = new RamHive("test_user_id");
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SubPhases(2));
            Assert.True(new SubPhases.Has(phase).Value());
            Assert.Equal(2, new SubPhases.RepeatCount(phase).Value());
        }

        [Fact]
        public void RetrievesSubPhase()
        {
            var hive = new RamHive("test_user_id");
            var comb = hive.Comb("test_program_id");
            var sub1 = new PhaseOf(comb, "sub_phase_1");
            sub1.Update(new Name("sub1"));
            var sub2 = new PhaseOf(comb, "sub_phase_2");
            var phase = new PhaseOf(comb, "test_phase_id");
            phase.Update(new SubPhases(3, sub1, sub2));

            Assert.Equal(
                "sub1",
                new Name.Of(
                    new PhaseOf(
                        comb,
                        new SubPhases.IDs(phase).First()
                    )
                ).AsString()
            );
        }
    }
}

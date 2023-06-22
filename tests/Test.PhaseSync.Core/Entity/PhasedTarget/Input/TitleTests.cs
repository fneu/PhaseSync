using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class TitleTests
    {

        [Fact]
        public void SetsTitle()
        {
            var target = new PhasedTargetOf(new RamHive("test_user_id"), "test_target_id");
            target.Update(new Title("TRAINING_ECONOMY"));

            Assert.True(new Title.Has(target).Value());
            Assert.Equal("Economy Run", new Title.Of(target).Value());
        }
    }
}

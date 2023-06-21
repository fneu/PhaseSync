using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class DescriptionTests
    {
        [Fact]
        public void SetsTitle()
        {
            var target = new PhasedTargetOf(new RamHive("test_user_id"), "test_target_id");
            target.Update(new Description("1h 4min", "12.3km", "---", "stuff"));

            Assert.True(new Description.Has(target).Value());
            Assert.Equal("1h 4min\n12.3km\n---\nstuff", new Description.Of(target).Value());
        }
    }
}

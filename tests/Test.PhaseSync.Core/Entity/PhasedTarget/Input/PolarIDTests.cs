using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class PolarIDTests
    {

        [Fact]
        public void SetsPolarID()
        {
            var target = new PhasedTargetOf(new RamHive("test_user_id"), "test_target_id");
            target.Update(new PolarID(123));
            Assert.Equal(123, new PolarID.Of(target).Value());
        }
    }
}

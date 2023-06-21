using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class TimeTests
    {

        [Fact]
        public void SetsLocalTime()
        {
            var target = new PhasedTargetOf(new RamHive("test_user_id"), "test_target_id");
            target.Update(new Time("2023-06-25T07:00:00Z", "Europe/Berlin"));
            Assert.Equal("2023-06-25T09:00", new Time.Of(target).Value());
        }
    }
}

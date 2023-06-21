using PhaseSync.Core.Units;

namespace Test.PhaseSync.Core.Units
{
    public sealed class HumanReadableDurationTests
    {
        [Fact]
        public void ConvertsShortDuration()
        {
            Assert.Equal("43min", new HumanReadableDuration(2580).AsString());
        }

        [Fact]
        public void ConvertsLongDuration()
        {
            Assert.Equal("1h 23min", new HumanReadableDuration(5000).AsString());
        }
    }
}

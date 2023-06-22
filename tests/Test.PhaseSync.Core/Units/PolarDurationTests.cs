using PhaseSync.Core.Units;

namespace Test.PhaseSync.Core.Units
{
    public sealed class PolarDurationTests
    {
        [Fact]
        public void ConvertsShortDuration()
        {
            Assert.Equal("00:43:01", new PolarDuration(2581).AsString());
        }

        [Fact]
        public void ConvertsLongDuration()
        {
            Assert.Equal("01:23:20", new PolarDuration(5000).AsString());
        }
    }
}

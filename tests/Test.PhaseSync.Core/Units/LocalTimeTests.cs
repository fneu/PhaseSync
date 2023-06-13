using PhaseSync.Core.Units;

namespace Test.PhaseSync.Core.Units
{
    public sealed class LocalTimeTests
    {
        [Fact]
        public void ConvertsToBerlinTime()
        {
            Assert.Equal("2023-06-25T09:00", new LocalTime("2023-06-25T07:00:00Z", "Europe/Berlin").Value());
        }
    }
}

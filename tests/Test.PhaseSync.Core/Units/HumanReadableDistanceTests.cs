using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Units;
using Xive.Props;

namespace Test.PhaseSync.Core.Units
{
    public sealed class HumanReadableDistanceTests
    {
        [Fact]
        public void ConvertsMetricDistance()
        {
            var settings = new SettingsOf(new RamProps());
            settings.Update(new ZoneUnit("METRIC"));

            Assert.Equal("1.0 km", new CompactHumanReadableDistance(1000, settings).AsString());
        }

        [Fact]
        public void ConvertsImperialDistance()
        {
            var settings = new SettingsOf(new RamProps());
            settings.Update(new ZoneUnit("IMPERIAL"));

            Assert.Equal("1.2 mi", new CompactHumanReadableDistance(2000, settings).AsString());
        }
    }
}

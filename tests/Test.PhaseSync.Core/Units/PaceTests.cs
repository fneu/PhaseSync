using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xive.Hive;

namespace Test.PhaseSync.Core.Units
{
    public sealed class PaceTests
    {
        [Fact]
        public void ConvertsToKM()
        {
            var settings = new SettingsOf(new RamHive("test_user_id"));
            settings.Update(new ZoneUnit("METRIC"));
            Assert.Equal("4:36", new Pace(3.625, settings).AsString());
        }

        [Fact]
        public void ConvertsToMiles()
        {
            Assert.Equal("7:24", new Pace(3.625, false).AsString());
        }
    }
}

using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Test.PhaseSync.Datum.Datum;
using Xive;
using Xive.Hive;
using Yaapii.Atoms.Text;

namespace Test.PhaseSync.Core.Entity.PhasedTarget
{
    public sealed class TAOTargetTests
    {
        [Theory]
        [InlineData("3.2k.json")]
        [InlineData("economy.json")]
        [InlineData("interval.json")]
        [InlineData("progression.json")]
        [InlineData("race.json")]
        public void CreatesWithoutError(string fileName)
        {
            var hive = new RamHive("test_user_id");
            var comb = hive.Comb("test_target_id");
            var settings = new SettingsOf(hive);
            settings.Update(
                new ZoneUnit("METRIC"));
            var workout = new TextOf(new DatumOf(fileName)).AsString();
            var target = new TAOTarget(new RamHive("test_user_id"), workout);
            foreach (var phase in new Phases.Of(target))
            {
                var content = new PhaseAsPolarJson(phase, comb, settings).Value();
            };
        }
    }
}

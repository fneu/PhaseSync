using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Phase
{
    public sealed class PhaseAsJsonTests
    {
        [Fact]
        public void convertsRepeatPhaseToJson()
        {
            var hive = new RamHive("test_user_id");
            var comb = hive.Comb("test_program_id");

            var settings = new SettingsOf(hive);
            settings.Update(
                new SetZones(true),
                new ZoneRadius(0.4),
                new ZoneLowerBounds(new double[] { 1, 2, 3, 4, 5 }),
                new ZoneUnit("METRIC")
            );

            var sub1 = new PhaseOf(comb, "sub_phase_1");
            sub1.Update(
                new Name("sub1"),
                new PhaseChangeType("MANUAL")
                );

            var sub2 = new PhaseOf(comb, "sub_phase_2");
            sub2.Update(
                new Name("sub2"),
                new SpeedGoal(3.2),
                new DistanceGoal(3200)
                );

            var repeat = new PhaseOf(comb, "repeat_phase");
            repeat.Update(
                new SubPhases(3, sub1, sub2)
            );

            var actual = new PhaseAsJson(repeat, comb, settings).Value().ToString();
            var expected = new JsonObject()
            {
                ["phaseType"] = "REPEAT",
                ["repeatCount"] = 3,
                ["phases"] = new JsonArray(
                    new JsonObject()
                    {
                        ["id"] = null,
                        ["lowerZone"] = null,
                        ["upperZone"] = null,
                        ["intensityType"] = null,
                        ["phaseChangeType"] = "MANUAL",
                        ["goalType"] = "DURATION",
                        ["duration"] = "00:01:00",
                        ["distance"] = null,
                        ["name"] = "sub1",
                        ["phaseType"] = "PHASE"
                    },
                    new JsonObject()
                    {
                        ["id"] = null,
                        ["lowerZone"] = 2,
                        ["upperZone"] = 3,
                        ["intensityType"] = "SPEED_ZONES",
                        ["phaseChangeType"] = "AUTOMATIC",
                        ["goalType"] = "DISTANCE",
                        ["duration"] = "00:00:00",
                        ["distance"] = 3200.0,
                        ["name"] = "sub2",
                        ["phaseType"] = "PHASE"
                    }
                )
            }.ToString();

            Assert.Equal(expected, actual);
        }
    }
}

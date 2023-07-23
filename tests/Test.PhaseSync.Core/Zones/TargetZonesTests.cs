using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Zones;
using Test.PhaseSync.Datum.Datum;
using Xive;
using Xive.Hive;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Text;

namespace Test.PhaseSync.Core.Zones
{
    public sealed class TargetZonesTests
    {
        [Fact]
        public void CalculatesTargetsZones()
        {

            var hive = new RamHive("test_user_id");
            var target = new PhasedTargetOf(hive, "test_target_id");

            var settings = new SettingsOf(hive);
            settings.Update(
                new ZoneRadius(0.6)
            );

            var phase1 = new PhaseOf(target.Memory(), "phase1");
            phase1.Update(new SpeedGoal(2));

            var phase2 = new PhaseOf(target.Memory(), "phase2");
            phase2.Update(new SpeedGoal(1));

            var phase3 = new PhaseOf(target.Memory(), "phase3");
            phase3.Update(new SpeedGoal(3));

            var phase4 = new PhaseOf(target.Memory(), "phase4");
            phase4.Update(new SpeedGoal(2));

            var repeatPhase = new PhaseOf(target.Memory(), "repeat");
            repeatPhase.Update(new SubPhases(3, phase2, phase3));

            target.Update(new Phases(new ManyOf<IEntity<IXocument>>(phase1, repeatPhase, phase4)));

            Assert.Equal(
                new double[] { 0.4, 1.4, 1.6, 2.4, 2.6 },
                new Mapped<IZone, double>(
                    zone => zone.Min(),
                    new TargetZones(target, settings)
                ).ToArray()
            );
        }

        [Theory]
        [InlineData("3.2k.json")]
        [InlineData("economy.json")]
        [InlineData("interval.json")]
        [InlineData("progression.json")]
        [InlineData("race.json")]
        public void WorkoutZones(string fileName)
        {
            var hive = new RamHive("test_user_id");
            var comb = hive.Comb("test_target_id");
            var settings = new SettingsOf(hive);
            settings.Update(
                new ZoneUnit("METRIC"),
                new ZoneRadius(0.15));

            var workout = new TextOf(new DatumOf(fileName)).AsString();
            var target = new TAOTarget(new RamHive("test_user_id"), workout);
            var zones =
                new Mapped<IZone, double>(
                    zone => zone.Min(),
                    new TargetZones(target, settings)
                ).ToArray();
        }
    }
}

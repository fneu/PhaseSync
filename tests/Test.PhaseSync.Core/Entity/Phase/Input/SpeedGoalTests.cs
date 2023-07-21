using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Phase.Input
{
    public sealed class SpeedGoalTests
    {
        [Fact]
        public void SetsSpeedGoal()
        {
            var hive = new RamHive("test_user_id");
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SpeedGoal(3.5));
            Assert.True(new SpeedGoal.Has(phase).Value());
        }

        [Fact]
        public void RetrievesSpeedGoal()
        {
            var hive = new RamHive("test_user_id");
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SpeedGoal(3.5));
            Assert.Equal(3.5, new SpeedGoal.InMPS(phase).Value());
        }

        [Fact]
        public void DeterminesLowerZones()
        {
            var hive = new RamHive("test_user_id");
            var settings = new SettingsOf(hive);
            settings.Update(
                new SetZones(true),
                new ZoneRadius(1),
                new ZoneLowerBounds(new double[] { 1, 2, 3, 4, 5}));
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SpeedGoal(3.5));
            Assert.Equal(2, new SpeedGoal.LowerZone(phase, settings).Value());
        }

        [Fact]
        public void FallbackLowerZones()
        {
            var hive = new RamHive("test_user_id");
            var settings = new SettingsOf(hive);
            settings.Update(
                new SetZones(false),
                new ZoneRadius(1),
                new ZoneLowerBounds(new double[] { 1, 2, 3, 4, 5}));
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SpeedGoal(3.5));
            Assert.Equal(1, new SpeedGoal.LowerZone(phase, settings).Value());
        }

        [Fact]
        public void DeterminesUpperZone()
        {
            var hive = new RamHive("test_user_id");
            var settings = new SettingsOf(hive);
            settings.Update(
                new SetZones(true),
                new ZoneRadius(1),
                new ZoneLowerBounds(new double[] { 1, 2, 3, 4, 5}));
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SpeedGoal(3.5));
            Assert.Equal(4, new SpeedGoal.UpperZone(phase, settings).Value());
        }

        [Fact]
        public void FallbackUpperZone()
        {
            var hive = new RamHive("test_user_id");
            var settings = new SettingsOf(hive);
            settings.Update(
                new SetZones(false),
                new ZoneRadius(1),
                new ZoneLowerBounds(new double[] { 1, 2, 3, 4, 5}));
            var phase = new PhaseOf(hive.Comb("test_program_id"), "test_phase_id");
            phase.Update(new SpeedGoal(3.5));
            Assert.Equal(5, new SpeedGoal.UpperZone(phase, settings).Value());
        }


    }
}

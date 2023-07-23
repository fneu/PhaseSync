using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Zones.Facets;
using Xive;
using Xive.Hive;
using Yaapii.Atoms.Enumerable;

namespace Test.PhaseSync.Core.Zones.Facets
{
    public sealed class TargetSpeedsTests
    {
        [Fact]
        public void ExtractsSpeeds()
        {
            var target = new PhasedTargetOf(new RamHive("test_user_id"), "test_target_id");

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
                new double[] { 1, 2, 3 },
                new TargetSpeeds(target).ToArray()
            );
        }
    }
}

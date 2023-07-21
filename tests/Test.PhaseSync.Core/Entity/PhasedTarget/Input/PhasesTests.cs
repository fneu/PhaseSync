using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using Xive;
using Xive.Hive;
using Yaapii.Atoms.Enumerable;

namespace Test.PhaseSync.Core.Entity.PhasedTarget.Input
{
    public sealed class PhasesTests
    {
        [Fact]
        public void SetsPhases()
        {
            var target = new PhasedTargetOf(new RamHive("test_user_id"), "test_target_id");

            var phase1 = new PhaseOf(target.Memory(), "phase1");
            phase1.Update(new Name("First Phase"));

            var phase2 = new PhaseOf(target.Memory(), "phase2");
            phase2.Update(new Name("Second Phase"));

            Assert.False(new Phases.Has(target).Value());

            target.Update(
                new Phases(
                    new ManyOf<IEntity<IXocument>>(phase1, phase2)
                )
            );

            Assert.Equal(
                "Second Phase",
                new Name.Of(new Phases.Of(target)[1]).AsString()
            );
        }
    }
}

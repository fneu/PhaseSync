using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using System.Text.Json.Nodes;
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
            Assert.False(new Phases.Has(target).Value());

            target.Update(
                new Phases(
                    new ManyOf<JsonNode>(
                        JsonNode.Parse("{\"name\": \"value\"}")!
                    )
                )
            );
            Assert.Equal("[\r\n  {\r\n    \"name\": \"value\"\r\n  }\r\n]", new Phases.Of(target).Value().ToString());
        }
    }
}

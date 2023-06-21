using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.PhasedTarget.Input
{

    public sealed class Phases : EntityInputEnvelope<IHoneyComb>
    {
        private const string KEY = "phases";

        public Phases(IEnumerable<JsonNode> phases) : base(
            comb => comb.Cell(KEY).Update(
                new InputOf(
                    new JsonArray(
                        phases.ToArray()
                    ).ToJsonString()
                )
            )

        )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IHoneyComb> target) : base(
                () => target.Memory().Cell(KEY).Content().Length != 0
            )
            { }
        }

        public sealed class Of : ScalarEnvelope<JsonNode>
        {
            public Of(IEntity<IHoneyComb> target) : base(
                () => JsonNode.Parse(
                    new TextOf(
                        target.Memory().Cell(KEY).Content()
                    ).AsString()
                )!
            )
            { }
        }
    }
}

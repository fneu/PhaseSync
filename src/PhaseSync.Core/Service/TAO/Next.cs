using System.Text.Json.Nodes;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Service.TAO
{
    public sealed class Next : ScalarEnvelope<JsonNode>
    {
        public Next(ISession taoSession) : base(() =>
            taoSession.Get("/api/mobile/plannedWorkouts")[0]!
        )
        { }
    }
}

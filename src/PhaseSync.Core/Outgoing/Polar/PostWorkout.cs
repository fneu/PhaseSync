using System.Text.Json.Nodes;

namespace PhaseSync.Core.Outgoing.Polar
{
    public sealed class PostWorkout : PostRequestEnvelope
    {
        public const string url = "/api/trainingtarget";
        public PostWorkout(JsonNode content) : base(url, content)
        { }
    }
}

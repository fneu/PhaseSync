using System.Text.Json.Nodes;

namespace PhaseSync.Core.Service
{
    public interface ISession
    {
        JsonNode Get(string url);
        JsonNode Post(string url, JsonNode content);
    }
}

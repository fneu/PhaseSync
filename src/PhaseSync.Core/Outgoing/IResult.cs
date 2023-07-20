using System.Text.Json.Nodes;

namespace PhaseSync.Core.Outgoing
{
    public interface IResult
    {
        bool Success();
        string ErrorMsg();
        JsonNode Content();
    }
}

using System.Text.Json.Nodes;

namespace PhaseSync.Core.Outgoing
{
    internal class ResultOf : IResult
    {
        private readonly bool success;
        private readonly string errorMsg;
        private readonly JsonNode content;

        public ResultOf(bool success, string errorMsg, JsonNode content)
        {
            this.success = success;
            this.errorMsg = errorMsg;
            this.content = content;
        }
        public JsonNode Content()
        {
            return content;
        }

        public string ErrorMsg()
        {
            return errorMsg;
        }

        public bool Success()
        {
            return success;
        }
    }
}

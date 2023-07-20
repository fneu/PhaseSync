namespace PhaseSync.Core.Outgoing
{
    public interface IRequest
    {
        Task<IResult> Send(HttpClient client);
    }
}

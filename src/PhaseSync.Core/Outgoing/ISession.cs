namespace PhaseSync.Core.Outgoing
{
    public interface ISession
    {
        Task<IResult> Send(IRequest request);
    }
}

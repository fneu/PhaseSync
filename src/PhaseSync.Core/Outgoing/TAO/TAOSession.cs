using System.Net.Http.Headers;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Outgoing.TAO
{
    public sealed class TAOSession : ISession, IDisposable
    {
        private const string baseAddress = "https://beta.trainasone.com";
        private readonly IScalar<HttpClient> client;

        public TAOSession(string token)
        {
            client = new ScalarOf<HttpClient>(() =>
            {
                var client = new HttpClient() { BaseAddress = new Uri(baseAddress) };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            });
        }

        public void Dispose()
        {
            client.Value().Dispose();
        }

        public async Task<IResult> Send(IRequest request)
        {
            return await request.Send(client.Value());
        }
    }
}

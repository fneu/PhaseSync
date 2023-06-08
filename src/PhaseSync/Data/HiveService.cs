using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using PhaseSync.Options;

namespace PhaseSync.Data
{
    public sealed class HiveService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly HiveServiceOptions options;

        public HiveService(AuthenticationStateProvider authenticationStateProvider, IOptions<HiveServiceOptions> options)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.options = options.Value;
        }

        public async Task<string> GetUserID()
        {
            var authstate = await this.authenticationStateProvider.GetAuthenticationStateAsync();
            if (!authstate.User.Identity!.IsAuthenticated)
            {
                return "not authenticated";
            }
            return authstate.User.Claims.First().Value + " --- " + options.Directory;
        }
    }
}

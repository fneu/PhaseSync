using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using PhaseSync.Blazor.Options;
using Xive;
using Xive.Hive;

namespace PhaseSync.Blazor.Data
{
    public sealed class HiveService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly PhaseSyncOptions options;

        public HiveService(AuthenticationStateProvider authenticationStateProvider, IOptions<PhaseSyncOptions> options)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.options = options.Value;
        }

        public async Task<IHive> UserHive()
        {
            try
            {
                var authstate = await this.authenticationStateProvider.GetAuthenticationStateAsync();
                return new FileHive(options.HiveDirectory, authstate.User.Claims.First().Value);
            }
            catch (Exception)
            {
                return new RamHive("");
            }
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using PhaseSync.Blazor.Data;
using PhaseSync.Blazor.Options;

namespace PhaseSync.Blazor.Pages
{

    public partial class Settings : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        public HiveService HiveService { get; set; } = default!;

        [Inject]
        public IOptions<PhaseSyncOptions> PhaseSyncOptions { get; set; }

        public string UserID { get; set; } = "initial";
        public string HiveID { get; set; } = "initial";

        protected async override Task OnInitializedAsync()
        {
            var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!authstate.User.Identity!.IsAuthenticated)
            {
                UserID = "not authenticated";
                return;
            }
            UserID = authstate.User.Claims.First().Value;
            HiveID = PhaseSyncOptions.Value.PasswordEncryptionSecret;
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace PhaseSync.Pages
{

    public partial class Settings : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        public string UserID { get; set; } = "initial";

        protected async override Task OnInitializedAsync()
        {
            var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!authstate.User.Identity!.IsAuthenticated)
            {
                UserID = "not authenticated";
                return;
            }
            UserID = authstate.User.Claims.First().Value;
        }
    }
}

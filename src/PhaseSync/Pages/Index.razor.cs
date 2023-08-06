
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace PhaseSync.Blazor.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected async override Task OnInitializedAsync()
        {
            try
            {
                var authstate = await this.authenticationStateProvider.GetAuthenticationStateAsync();
                var authenticated = authstate.User.Identity?.IsAuthenticated ?? false;
                if (authenticated)
                {
                    NavigationManager.NavigateTo("/workouts");

                }
            }
            catch (Exception) { }
        }
    }
}

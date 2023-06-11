using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PhaseSync.Blazor.Data;

namespace PhaseSync.Blazor.Pages
{

    public partial class Settings : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        public HiveService HiveService { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

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
            HiveID = await HiveService.GetUserID();
        }

        public void TAOAuthentication()
        {
            var parameters = new Dictionary<string, string>
            {
                {"response_type", "code" },
                {"client_id", "PhaseSync" },
                {"redirect_uri", "http://localhost/auth/trainasone" },
                {"scope", "WORKOUT" },
                {"state", "1234zyx" }
            };

            var url = new UriBuilder("https://beta.trainasone.com/oauth/authorise")
            {
                Query = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))
            }.ToString();

            NavigationManager.NavigateTo(url);
        }
    }
}

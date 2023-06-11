using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using PhaseSync.Blazor.Data;
using PhaseSync.Blazor.Options;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using System.Text.Json;

namespace PhaseSync.Blazor.Pages.Auth
{
    public partial class TrainAsONE : ComponentBase
    {
        [Inject]
        public IOptions<PhaseSyncOptions> Options { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public HiveService HiveService { get; set; } = default!;

        public string? Error { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var code)
                && (QueryHelpers.ParseQuery(uri.Query).TryGetValue("state", out var state)))
            {
                var settings = new SettingsOf(await HiveService.UserHive());

                if (state[0] != new TaoState.Of(settings).Value())
                {
                    Error = "Random State does not match";
                    return;
                }

                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://beta.trainasone.com");
                var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/token");

                var parameters = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", code[0]!},
                    {"client_id", "PhaseSync"},
                    {"client_secret", Options.Value.TAOClientSecret },
                    {"redirect_uri", "http://localhost/auth/trainasone"}
                };
                request.Content = new FormUrlEncodedContent(parameters);

                var response =  httpClient.Send(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    var token = data!["access_token"].ToString();
                    settings.Update(new TaoToken(token!));
                    NavigationManager.NavigateTo("/settings");

                } else
                {
                    Error = "Something went wrong";

                }
            }
            else
            {
                Error = "Something went wrong";
            }
        }
    }
}

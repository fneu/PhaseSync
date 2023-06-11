using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace PhaseSync.Blazor.Pages.Auth
{
    public partial class TrainAsONE : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        public string? Code { get; set; }
        public string? State { get; set; }
        public string? Error { get; set; }

        protected override void OnInitialized()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var code)
                && (QueryHelpers.ParseQuery(uri.Query).TryGetValue("state", out var state)))
            {
                Code = code;
                State = state;

                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://beta.trainasone.com");
                var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/token");

                var parameters = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", Code!},
                    {"client_id", "PhaseSync"},
                    {"client_secret", ""},
                    {"redirect_uri", "http://localhost/auth/trainasone"}
                };
                request.Content = new FormUrlEncodedContent(parameters);

                var response =  httpClient.Send(request);

                if (response.IsSuccessStatusCode)
                {

                } else
                {

                }

            }
            else
            {
                Error = "Something went wrong";
            }
        }
    }
}

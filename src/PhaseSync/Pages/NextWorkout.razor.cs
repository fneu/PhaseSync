using Microsoft.AspNetCore.Components;
using PhaseSync.Blazor.Data;
using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using System.Net.Http.Headers;
using System.Text.Json;
using Xive;

namespace PhaseSync.Blazor.Pages
{
    public partial class NextWorkout : ComponentBase
    {
        [Inject]
        public HiveService HiveService { get; set; } = default!;

        public IEntity<IProps> UserSettings { get; set; } = default!;
        public bool TAOConnected { get; set; } = false;
        public string? Error { get; set; }

        public string Workout { get; set; } = "";

        protected async override Task OnInitializedAsync()
        {
            this.UserSettings = new SettingsOf(await HiveService.UserHive());
            this.TAOConnected = new TaoToken.Has(this.UserSettings).Value();

            if (TAOConnected)
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://beta.trainasone.com");
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/mobile/plannedWorkouts");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", new TaoToken.Of(UserSettings).Value());
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response =  httpClient.Send(request);

                if (response.IsSuccessStatusCode)
                {
                    Workout = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Error = await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}

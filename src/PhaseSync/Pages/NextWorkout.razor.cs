using Microsoft.AspNetCore.Components;
using PhaseSync.Blazor.Data;
using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Service;
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
                var taoSession = new TAOSession(new TaoToken.Of(UserSettings).Value());
                try
                {
                    Workout = taoSession.Get("/api/mobile/plannedWorkouts")[0].ToString();
                } catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }
        }
    }
}

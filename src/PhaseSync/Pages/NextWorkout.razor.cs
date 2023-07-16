using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using PhaseSync.Blazor.Data;
using PhaseSync.Blazor.Options;
using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Service;
using PhaseSync.Core.Service.TAO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Blazor.Pages
{
    public partial class NextWorkout : ComponentBase
    {
        [Inject]
        public HiveService HiveService { get; set; } = default!;

        [Inject]
        public IOptions<PhaseSyncOptions> PhaseSyncOptions { get; set; } = default!;

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        public IEntity<IProps> UserSettings { get; set; } = default!;
        public bool TAOConnected { get; set; } = false;
        public string? Error { get; set; }

        public JsonNode Workout { get; set; } = "";

        protected async override Task OnInitializedAsync()
        {
            this.UserSettings = new SettingsOf(await HiveService.UserHive());
            this.TAOConnected = new TaoToken.Has(this.UserSettings).Value();

            if (TAOConnected)
            {
                var taoSession = new TAOSession(new TaoToken.Of(UserSettings).Value());
                try
                {
                    Workout = new Next(taoSession).Value();
                } catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }
        }

        public void SendToPolar()
        {
            try
            {
                var hive = HiveService.UserHive().Result;
                var settings = new SettingsOf(hive);
                var target = new TAOTarget(hive, Workout);
                var polarJson = new JsonObject()
                {
                    ["type"] = "PHASED",
                    ["name"] = new Title.Of(target).Value(),
                    ["description"] = new Description.Of(target).Value(),
                    ["datetime"] = new Time.Of(target).Value(),
                    ["exerciseTargets"] = new JsonArray() {
                        new JsonObject()
                        {
                            ["id"] = null,
                            ["distance"] = null,
                            ["calories"] = null,
                            ["duration"] = null,
                            ["index"] = 0,
                            ["sportId"] = 1,
                            ["phases"] = new Phases.Of(target).Value()
                        }
                    }
                };
                var polarSession = 
                    new PolarSession(
                        new PolarEmail.Of(settings).Value(),
                        new PolarPassword.Of(settings, PhaseSyncOptions.Value.PasswordEncryptionSecret).Value());

                polarSession.Post("/api/trainingtarget", polarJson);
                Snackbar.Add("The workout was uploaded to polar flow!", Severity.Success);
            }
            catch (Exception ex){
                Snackbar.Add($"Upload failed: {ex.Message}", Severity.Error);
            }
        }
    }
}

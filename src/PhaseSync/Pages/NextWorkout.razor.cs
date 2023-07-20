﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using PhaseSync.Blazor.Data;
using PhaseSync.Blazor.Options;
using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Outgoing.Polar;
using PhaseSync.Core.Outgoing.TAO;
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
        public bool SettingsComplete { get; set; } = false;
        public string? Error { get; set; }

        public JsonNode Workout { get; set; } = "";

        protected async override Task OnInitializedAsync()
        {
            this.UserSettings = new SettingsOf(await HiveService.UserHive());
            this.TAOConnected = new TaoToken.Has(this.UserSettings).Value();
            this.SettingsComplete = new SettingsComplete.Of(this.UserSettings).Value();

            if (TAOConnected)
            {
                var taoSession = new TAOSession(new TaoToken.Of(UserSettings).Value());
                var workoutResult = await taoSession.Send(new GetUpcomingWorkout());
                if (workoutResult.Success()){
                    Workout = workoutResult.Content();
                }
                else
                {
                    Error = workoutResult.ErrorMsg();
                }
            }
        }

        public async void SendToPolar()
        {
            try
            {
                var hive = HiveService.UserHive().Result;
                var settings = new SettingsOf(hive);
                var target = new TAOTarget(hive, Workout.ToString());
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

                var result = await polarSession.Send(new PostWorkout(polarJson));
                if (result.Success())
                {
                    Snackbar.Add("The workout was uploaded to polar flow!", Severity.Success);
                }
                else
                {
                    Snackbar.Add($"Upload failed: {result.ErrorMsg()}", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Sending to Polar failed: {ex.Message}", Severity.Error);
            }
        }
    }
}

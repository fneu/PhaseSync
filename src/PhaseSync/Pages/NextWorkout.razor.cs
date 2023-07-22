using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using PhaseSync.Blazor.Data;
using PhaseSync.Blazor.Options;
using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Outgoing.Polar;
using PhaseSync.Core.Outgoing.TAO;
using PhaseSync.Core.Zones;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Enumerable;

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

        public JsonNode? Workout { get; set; }

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
                var polarSession =
                    new PolarSession(
                        new PolarEmail.Of(settings).Value(),
                        new PolarPassword.Of(settings, PhaseSyncOptions.Value.PasswordEncryptionSecret).Value());

                foreach (var existingTarget in new PhasedTargetCollection(hive))
                {
                    await polarSession.Send(new DeleteTarget(hive, existingTarget));
                }

                var target = new TAOTarget(hive, Workout!.ToString());

                var sportProfileResult = await polarSession.Send(new GetRunningProfile());
                if (sportProfileResult.Success())
                {
                    try
                    {
                        var zones = new TargetZones(target, settings);
                        var zonesResult = await polarSession.Send(new PostZones(zones, sportProfileResult.Content().ToString(), settings));
                        if (zonesResult.Success())
                        {
                            settings.Update(
                                new ZoneLowerBounds(
                                    new Mapped<IZone, double>(
                                        zone => zone.Min(),
                                        zones
                                    ).ToArray()
                                )
                            );
                            Snackbar.Add($"Speed zones were updated!", Severity.Success);
                        }
                        else
                        {
                            Snackbar.Add($"Settings zones failed: {zonesResult.ErrorMsg()}", Severity.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        Snackbar.Add($"Calculating zones failed: {ex.Message}", Severity.Warning);
                    }
                }
                else
                {
                    Snackbar.Add($"Getting running profile failed: {sportProfileResult.ErrorMsg()}", Severity.Warning);
                }

                var result = await polarSession.Send(new PostTarget(target, settings));
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

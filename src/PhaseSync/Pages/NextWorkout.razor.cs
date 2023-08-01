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
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.ShapeLib;
using Plotly.Blazor.Traces.ScatterLib;
using Line = Plotly.Blazor.LayoutLib.ShapeLib.Line;
using System.Text.Json.Nodes;
using Xive;
using Yaapii.Atoms.Enumerable;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Plotly.Blazor.ConfigLib;
using Yaapii.Atoms.List;
using Xive.Hive;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Units;
using PhaseSync.Blazor.Models;

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

        [Inject]
        private IDialogService DialogService { get; set; }

        public IEntity<IProps> UserSettings { get; set; } = default!;
        public bool TAOConnected { get; set; } = false;
        public bool SettingsComplete { get; set; } = false;
        public string? Error { get; set; }

        public IEnumerable<UIWorkout>? Workouts { get; set; }

        Config config = new()
        {
            Responsive = true,
            StaticPlot=true,
            DisplayModeBar=DisplayModeBarEnum.False
        };


        protected async override Task OnInitializedAsync()
        {
            this.UserSettings = new SettingsOf(await HiveService.UserHive());
            this.TAOConnected = new TaoToken.Has(this.UserSettings).Value();
            this.SettingsComplete = new SettingsComplete.Of(this.UserSettings).Value();

            if (TAOConnected)
            {
                var taoSession = new TAOSession(new TaoToken.Of(UserSettings).Value());
                var workoutResultArray = await taoSession.Send(new GetUpcomingWorkouts());
                if (workoutResultArray.Success())
                {
                    this.Workouts = new Yaapii.Atoms.Enumerable.Mapped<JsonNode, UIWorkout>(
                        json => new UIWorkout(json, this.UserSettings),
                        workoutResultArray.Content().AsArray()!
                    );
                }
                else
                {
                    Error = workoutResultArray.ErrorMsg();
                }
            }
        }

        public async Task SendToPolar(UIWorkout workout)
        {
            try
            {
                var hive = HiveService.UserHive().Result;
                var settings = new SettingsOf(hive);
                var polarSession =
                    new PolarSession(
                        new PolarEmail.Of(settings).Value(),
                        new PolarPassword.Of(settings, PhaseSyncOptions.Value.PasswordEncryptionSecret).Value());

                var target = new TAOTarget(new RamHive(""), workout.Workout);

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
                                    new Yaapii.Atoms.Enumerable.Mapped<IZone, double>(
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

                if (new EnableSync.Of(settings).Value() && new SetZones.Of(settings).Value())
                {
                    await DialogService.ShowMessageBox(
                        "Warning", 
                        (MarkupString) "Background sync and setting of zones is enabled. This will <b>regularily overwrite your Polar speed zones</b> according to the next upcoming workout, affecting this manually uploaded workout.<br>Consider disabling background sync in settings!", 
                        yesText:"OK"
                    );
                } else if (new EnableSync.Of(settings).Value())
                {
                    await DialogService.ShowMessageBox(
                        "Warning",
                        (MarkupString) "Background sync is enabled. This manually uploaded workout will not be overwritten, but additional versions of this workout will be synced once the start time expected by TrainAsONE changes.<br>Consider disabling background sync in settings!",
                        yesText: "OK"
                    );
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Sending to Polar failed: {ex.Message}", Severity.Error);
            }
        }
    }
}

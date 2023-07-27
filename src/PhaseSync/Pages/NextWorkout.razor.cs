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

        Config config = new()
        {
            Responsive = true,
            StaticPlot=true,
            DisplayModeBar=DisplayModeBarEnum.False
        };

        Layout layout = new()
        {
            XAxis = new List<XAxis>
            {
                new XAxis
                {
                    Title = new Plotly.Blazor.LayoutLib.XAxisLib.Title
                    {
                        Text = "Minutes"
                    },
                    NTicks = 4,
                    ShowGrid=false
                }
            },
            YAxis = new List<YAxis>
            {
                new YAxis
                {
                    Title = new Plotly.Blazor.LayoutLib.YAxisLib.Title
                    {
                        Text = "Paces"
                    },
                    ShowGrid=true
                },
                new YAxis
                {
                    Title = new Plotly.Blazor.LayoutLib.YAxisLib.Title
                    {
                        Text = "Zones",
                    },
                    Side = Plotly.Blazor.LayoutLib.YAxisLib.SideEnum.Right,
                    Matches = "y",
                    ShowTickLabels=true,
                    Overlaying = "y",
                    ShowGrid=false,
                },
            },
            Height = 400,
            ShowLegend = false,
        };

        List<ITrace> data = new() { };

        protected async override Task OnInitializedAsync()
        {
            this.UserSettings = new SettingsOf(await HiveService.UserHive());
            this.TAOConnected = new TaoToken.Has(this.UserSettings).Value();
            this.SettingsComplete = new SettingsComplete.Of(this.UserSettings).Value();

            if (TAOConnected)
            {
                var taoSession = new TAOSession(new TaoToken.Of(UserSettings).Value());
                var workoutResult = await taoSession.Send(new GetUpcomingWorkout());
                if (workoutResult.Success())
                {
                    Workout = workoutResult.Content();

                    var hive = HiveService.UserHive().Result;
                    var realSettings = new SettingsOf(hive);
                    var ramHive = new RamHive("");
                    var target = new TAOTarget(ramHive, Workout!.ToString());
                    var zones = new TargetZones(target, realSettings);

                    var totalDuration = 0;
                    var segmentStarts = new List<double>();
                    var segmentEnds = new List<double>();
                    var segmentMins = new List<int>();
                    var segmentMaxs = new List<int>();
                    var segmentVelocities = new List<double>();
                    var segmentVelocitiesObj = new List<object>();

                    foreach (var phase in new Phases.Of(target))
                    {
                        if (new SubPhases.Has(phase).Value())
                        {
                            for (int i = 0; i < new SubPhases.RepeatCount(phase).Value(); i++)
                            {
                                foreach (var subPhaseID in new SubPhases.IDs(phase))
                                {
                                    var subPhase = new PhaseOf(target.Memory(), subPhaseID);
                                    segmentStarts.Add(totalDuration/60);
                                    totalDuration += new Duration.InSeconds(subPhase).Value();
                                    segmentEnds.Add(totalDuration/60);
                                    segmentMins.Add(new SpeedGoal.Has(subPhase).Value() ? new SpeedGoal.LowerZone(subPhase, realSettings).Value() : -1);
                                    segmentMaxs.Add(new SpeedGoal.Has(subPhase).Value() ? new SpeedGoal.UpperZone(subPhase, realSettings).Value() : -1);
                                    segmentVelocities.Add(new Velocity.InMPS(subPhase).Value());
                                    segmentVelocitiesObj.Add(new Velocity.InMPS(subPhase).Value());
                                }
                            }
                        }
                        else
                        {
                            segmentStarts.Add(totalDuration/60);
                            totalDuration += new Duration.InSeconds(phase).Value();
                            segmentEnds.Add(totalDuration/60);
                            segmentMins.Add(new SpeedGoal.Has(phase).Value() ? new SpeedGoal.LowerZone(phase, realSettings).Value() : -1);
                            segmentMaxs.Add(new SpeedGoal.Has(phase).Value() ? new SpeedGoal.UpperZone(phase, realSettings).Value() : -1);
                            segmentVelocities.Add(new Velocity.InMPS(phase).Value());
                            segmentVelocitiesObj.Add(new Velocity.InMPS(phase).Value());
                        }
                    }

                    var colors = new List<string>() { "#72e88b", "#37dbdb", "#08ccf9", "#0face7", "#1280db" };
                    var shapes = new List<Shape>();
                    // zones
                    for (int s = 0; s < segmentStarts.Count; s++)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            shapes.Add(
                                new Shape
                                {
                                    Type = TypeEnum.Rect,
                                    XRef = "x",
                                    YRef = "y2",
                                    X0 = segmentStarts[s],
                                    Y0 = zones[i].Min(),
                                    X1 = segmentEnds[s],
                                    Y1 = zones[i].Max(),
                                    FillColor = colors[i],
                                    Opacity = ((segmentMins[s] <= i + 1) && (segmentMaxs[s] >= i + 1)) ? new decimal(0.45) : new decimal(0.15),
                                    Line = new Line
                                    {
                                        Width = 0,
                                    }
                                }
                            );
                        }
                    }
                    layout.Shapes = shapes;

                    var x = new List<object>() { };
                    var y = new List<object>() { };
                    for (int i = 0; i < segmentStarts.Count; i++)
                    {
                        x.Add(segmentStarts[i]);
                        y.Add(segmentVelocities[i]);
                        x.Add(segmentEnds[i]);
                        y.Add(segmentVelocities[i]);
                    }


                    data = new List<ITrace>()
                    {
                        new Scatter
                        {
                            Name = "Velocity",
                            Mode = ModeFlag.Lines,
                            X = x,
                            Y = y,
                            Line = new Plotly.Blazor.Traces.ScatterLib.Line
                            {
                                Color = "#2d4ed3",
                                Width = 2
                            },
                        },

                    };

                    layout.YAxis[0].Range = new List<object>() {
                        segmentVelocities.Min()-2*new ZoneRadius.Of(realSettings).Value(),
                        segmentVelocities.Max()+2*new ZoneRadius.Of(realSettings).Value()
                    };

                    layout.YAxis[0].TickMode = Plotly.Blazor.LayoutLib.YAxisLib.TickModeEnum.Array;
                    layout.YAxis[0].TickVals = segmentVelocitiesObj;
                    layout.YAxis[0].TickText = new List<object>(
                        new Yaapii.Atoms.Enumerable.Mapped<double, string>(
                            mps => new Pace(mps, realSettings).AsString(),
                            segmentVelocities)
                        );


                    layout.YAxis[1].TickMode = Plotly.Blazor.LayoutLib.YAxisLib.TickModeEnum.Array;
                    var zoneTicksDouble = new List<double>();
                    var zoneTicksObject = new List<object>();
                    foreach (var zone in zones)
                    {
                        zoneTicksDouble.Add(zone.Min());
                        zoneTicksDouble.Add(zone.Max());
                        zoneTicksObject.Add(zone.Min());
                        zoneTicksObject.Add(zone.Max());
                    }
                    layout.YAxis[1].TickVals = zoneTicksObject;
                    layout.YAxis[1].TickText = new List<object>(
                        new Yaapii.Atoms.Enumerable.Mapped<double, string>(
                            mps => new Pace(mps, realSettings).AsString(),
                            zoneTicksDouble)
                        );
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
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Sending to Polar failed: {ex.Message}", Severity.Error);
            }
        }
    }
}

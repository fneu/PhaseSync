using MudBlazor;
using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Phase;
using PhaseSync.Core.Entity.Phase.Input;
using PhaseSync.Core.Entity.PhasedTarget;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Units;
using PhaseSync.Core.Zones;
using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.ShapeLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;
using System.Globalization;
using System.Text.Json.Nodes;
using Xive;
using Xive.Hive;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Map;
using Line = Plotly.Blazor.LayoutLib.ShapeLib.Line;

namespace PhaseSync.Blazor.Models
{
    public class UIWorkout
    {
        public JsonNode Workout { get; set; } // done
        public string Icon { get; set; } // done
        public string Title { get; set; } // done
        public string NiceDate { get; set; } // done
        public string NiceDuration { get; set; } // done
        public string NiceDistance { get; set; } // done
        public Layout Layout { get; set; } // done
        public List<ITrace> Data { get; set; } // done
        public List<UIStep> Steps { get; set; }

        public UIWorkout(JsonNode workout, IEntity<IProps> realSettings)
        {
            this.Workout = workout;
            var ramHive = new RamHive("");
            var tempSettings = new SettingsOf(ramHive);
            tempSettings.Update(
                new ZoneUnit(new ZoneUnit.Of(realSettings).Value()),
                new SetZones(true),
                new ZoneRadius(new ZoneRadius.Of(realSettings).Value())
            );

            var target = new TAOTarget(ramHive, workout.ToString());
            var zones = new TargetZones(target, realSettings);
            tempSettings.Update(
                new ZoneLowerBounds(Yaapii.Atoms.List.Mapped.New(zone => zone.Min(), zones).ToArray())
            );

            float totalDuration = 0;
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
                            segmentMins.Add(new SpeedGoal.Has(subPhase).Value() ? new SpeedGoal.LowerZone(subPhase, tempSettings).Value() : -1);
                            segmentMaxs.Add(new SpeedGoal.Has(subPhase).Value() ? new SpeedGoal.UpperZone(subPhase, tempSettings).Value() : -1);
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
                    segmentMins.Add(new SpeedGoal.Has(phase).Value() ? new SpeedGoal.LowerZone(phase, tempSettings).Value() : -1);
                    segmentMaxs.Add(new SpeedGoal.Has(phase).Value() ? new SpeedGoal.UpperZone(phase, tempSettings).Value() : -1);
                    segmentVelocities.Add(new Velocity.InMPS(phase).Value());
                    segmentVelocitiesObj.Add(new Velocity.InMPS(phase).Value());
                }
            }

            var x = new List<object>() { };
            var y = new List<object>() { };
            for (int i = 0; i < segmentStarts.Count; i++)
            {
                x.Add(segmentStarts[i]);
                y.Add(segmentVelocities[i]);
                x.Add(segmentEnds[i]);
                y.Add(segmentVelocities[i]);
            }

            this.Data = new List<ITrace>()
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

            var colors = new List<string>() { "#72e88b", "#37dbdb", "#08ccf9", "#0face7", "#1280db" };
            var shapes = new List<Shape>();
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

            var zoneTicksDouble = new List<double>();
            var zoneTicksObject = new List<object>();
            foreach (var zone in zones)
            {
                zoneTicksDouble.Add(zone.Min());
                zoneTicksDouble.Add(zone.Max());
                zoneTicksObject.Add(zone.Min());
                zoneTicksObject.Add(zone.Max());
            }

            this.Layout = new()
            {
                XAxis = new List<XAxis>
                {
                    new XAxis
                    {
                        NTicks = 4,
                        ShowGrid = false
                    }
                },
                YAxis = new List<YAxis>
                {
                    new YAxis
                    {
                        ShowGrid = true,
                        Range = new List<object>() {
                            segmentVelocities.Min()-new ZoneRadius.Of(realSettings).Value(),
                            segmentVelocities.Max()+1.25*new ZoneRadius.Of(realSettings).Value()
                        },
                        TickMode = Plotly.Blazor.LayoutLib.YAxisLib.TickModeEnum.Array,
                        TickVals = segmentVelocitiesObj,
                        TickText = new List<object>(
                            new Yaapii.Atoms.Enumerable.Mapped<double, string>(
                                mps => new Pace(mps, realSettings).AsString(),
                                segmentVelocities
                            )
                        ),
                    },
                    new YAxis
                    {
                        Side = Plotly.Blazor.LayoutLib.YAxisLib.SideEnum.Right,
                        Matches = "y",
                        ShowTickLabels = true,
                        Overlaying = "y",
                        ShowGrid = false,

                        TickMode = Plotly.Blazor.LayoutLib.YAxisLib.TickModeEnum.Array,
                        TickVals = zoneTicksObject,
                        TickText = new List<object>(
                            new Yaapii.Atoms.Enumerable.Mapped<double, string>(
                                mps => new Pace(mps, realSettings).AsString(),
                                zoneTicksDouble
                            )
                        ),
                    }
                },
                ShowLegend = false,
                Margin = new Plotly.Blazor.LayoutLib.Margin
                {
                    L = 50,
                    R = 50,
                    B = 20,
                    T = 0,
                    Pad = 4
                },
                Shapes = shapes,
            };

            this.Icon = new FallbackMap(
                new MapOf(
                    new KvpOf("ASSESSMENT_3200_METRE", Icons.Material.Filled.AutoGraph),
                    new KvpOf("ASSESSMENT_6_MIN", Icons.Material.Filled.Insights),
                    new KvpOf("ASSESSMENT_PERCEIVED_EFFORT", Icons.Material.Filled.AutoAwesome),
                    new KvpOf("PARKRUN", Icons.Material.Filled.SportsScore),
                    new KvpOf("RACE_ROAD_BEST_EFFORT", Icons.Material.Filled.SportsScore),
                    new KvpOf("RACE_ROAD_CASUAL", Icons.Material.Filled.SportsScore),
                    new KvpOf("RACE_TRAIL_BEST_EFFORT", Icons.Material.Filled.SportsScore),
                    new KvpOf("RACE_TRAIL_CASUAL", Icons.Material.Filled.SportsScore),
                    new KvpOf("REST", Icons.Material.Filled.DirectionsWalk),
                    new KvpOf("RUN_WALK", Icons.Material.Filled.DirectionsWalk),
                    new KvpOf("TRAINING_ECONOMY", Icons.Material.Filled.LinearScale),
                    new KvpOf("TRAINING_ECONOMY_PLUS", Icons.Material.Filled.LinearScale),
                    new KvpOf("TRAINING_INTERVAL", Icons.Material.Filled.StackedLineChart),
                    new KvpOf("TRAINING_PICKUP", Icons.Material.Filled.LinearScale),
                    new KvpOf("TRAINING_PROGRESSION", Icons.Material.Filled.SignalCellularAlt),
                    new KvpOf("TRAINING_RECOVERY", Icons.Material.Filled.LinearScale),
                    new KvpOf("TRAINING_REPETITION", Icons.Material.Filled.StackedLineChart),
                    new KvpOf("TRAINING_RUN_WALK", Icons.Material.Filled.DirectionsWalk),
                    new KvpOf("TRAINING_TABATA", Icons.Material.Filled.StackedLineChart),
                    new KvpOf("TRAINING_THRESHOLD", Icons.Material.Filled.ShowChart)
                ),
                unknown => Icons.Material.Filled.DirectionsRun
            )[(string)Workout["activitySubType"]!];

            this.Title = new Core.Entity.PhasedTarget.Input.Title.Of(target).Value();


            this.NiceDuration = new CompactHumanReadableDuration(new ExpectedDurationS.Of(target).Value()).AsString();
            this.NiceDistance = new CompactHumanReadableDistance(new ExpectedDistanceM.Of(target).Value(), realSettings).AsString();

            var convertedTargetDate = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.ParseExact((string)workout["start"]!,
                    "yyyy-MM-ddTHH:mm:ssZ",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
                ),
                TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin") // TODO: Use Timezone here, too
            );
            var now = DateTime.UtcNow;

            if (convertedTargetDate.Date == now.Date)
            {
                this.NiceDate = "Today";
            }
            else if (convertedTargetDate.Date == now.AddDays(1).Date)
            {
                this.NiceDate = "Tomorrow";
            }
            else if (convertedTargetDate.Date < now.AddDays(7).Date)
            {
                this.NiceDate = convertedTargetDate.ToString("dddd", CultureInfo.InvariantCulture);
            }
            else
            {
                this.NiceDate = convertedTargetDate.ToString("dddd, MMM d", CultureInfo.InvariantCulture);
            }

            var steps = new List<UIStep>();
            foreach(var phase in new Phases.Of(target))
            {
                if(new SubPhases.Has(phase).Value())
                {
                    steps.Add(
                        new UIStep(
                            $"Repeat {new SubPhases.RepeatCount(phase).Value()} times:",
                            Mapped.New(
                                id => PhaseTitle(
                                    new PhaseOf(target.Memory(), id),
                                    realSettings
                                ),
                                new SubPhases.IDs(phase)
                            )
                        )
                    );

                }
                else
                {
                    steps.Add(new UIStep(PhaseTitle(phase, realSettings), new string[] { }));
                }
            }
            this.Steps = steps;
        }

        public string PhaseTitle(IEntity<IXocument> phase, IEntity<IProps> realSettings)
        {
            string duration;
            if(new DistanceGoal.Has(phase).Value())
            {
                duration = new HumanReadableDistance(new DistanceGoal.InMeters(phase).Value(), realSettings).AsString();
            }
            else
            {
                duration = new HumanReadableDuration(new Duration.InSeconds(phase).Value()).AsString();
            }

            return new FallbackMap(
                new MapOf(
                    new KvpOf("COOLDOWN", () => $"Cooldown for {duration}."),
                    new KvpOf("EXTREME_DISTANCE", () => $"Run {duration} in as QUICK a time as you can."),
                    new KvpOf("EXTREME_DURATION", () => $"Run as FAR as you can in {duration}."),
                    new KvpOf("PERCEIVED_CONVERSATIONAL", () => $"Run at perceived conversational pace for {duration}."),
                    new KvpOf("PERCEIVED_NATURAL", () => $"Run at perceived natural pace for {duration}."),
                    new KvpOf("PERCEIVED_WARMUP", () => $"Run at perceived warmup pace for {duration}."),
                    new KvpOf("RECOVERY", () => $"Start from a slow walking pace and increase speed gradually for {duration}."),
                    new KvpOf("STANDING", () => $"Stand still for {duration}.")
                ),
                pace => $"Run at a {pace}{(new ZoneUnit.Of(realSettings).Value() == "METRIC" ? "/km": "/mi")} pace for {duration}."
            )[new Name.Of(phase).AsString()];
        }
    }
}

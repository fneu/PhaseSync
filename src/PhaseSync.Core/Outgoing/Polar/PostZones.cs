using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Units;
using PhaseSync.Core.Zones;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Core.Outgoing.Polar
{
    public sealed class PostZones : PostRequestEnvelope
    {
        public PostZones(IList<IZone> zones, string sportProfileId, IEntity<IProps> settings) : base(
            "/settings/sports/save",
            new JsonObject
            {
                ["sports"] = new JsonObject
                {
                    [sportProfileId] = new JsonArray(
                        new JsonObject
                        {
                            ["name"] = "SpeedSettings",
                            ["settings"] = new JsonArray(
                                new JsonObject
                                {
                                    ["name"] = "measurementUnit",
                                    ["value"] = new ZoneUnit.Of(settings).Value()
                                },
                                new JsonObject
                                {
                                    ["name"] = "speedViewMode",
                                    ["value"] = new ZoneMode.Of(settings).Value()
                                },
                                new JsonObject
                                {
                                    ["name"] = "masEstimated",
                                    ["value"] = false
                                },
                                new JsonObject
                                {
                                    ["name"] = "mas",
                                    ["value"] = new Pace(new ZoneMas.Of(settings).Value(), settings).AsString()
                                },
                                new JsonObject
                                {
                                    ["name"] = "speedZoneType",
                                    ["value"] = "FREE"
                                },
                                new JsonObject
                                {
                                    ["name"] = "freeSpeedZone1Min",
                                    ["value"] = new KPHorMPH(zones[0].Min(), settings).AsString()
                                },
                                new JsonObject
                                {
                                    ["name"] = "freeSpeedZone2Min",
                                    ["value"] = new KPHorMPH(zones[1].Min(), settings).AsString()
                                },
                                new JsonObject
                                {
                                    ["name"] = "freeSpeedZone3Min",
                                    ["value"] = new KPHorMPH(zones[2].Min(), settings).AsString()
                                },
                                new JsonObject
                                {
                                    ["name"] = "freeSpeedZone4Min",
                                    ["value"] = new KPHorMPH(zones[3].Min(), settings).AsString()
                                },
                                new JsonObject
                                {
                                    ["name"] = "freeSpeedZone5Min",
                                    ["value"] = new KPHorMPH(zones[4].Min(), settings).AsString()
                                }
                            )
                        }
                    )
                }
            }
        )
        { }
    }
}

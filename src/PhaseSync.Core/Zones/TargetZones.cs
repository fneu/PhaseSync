using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings.Input;
using PhaseSync.Core.Zones.Facets;
using Xive;
using Yaapii.Atoms.List;

namespace PhaseSync.Core.Zones
{
    public sealed class TargetZones : ListEnvelope<IZone>
    {
        public const double FallbackSpeed = 2.78; // 6 min/km

        public TargetZones(IEntity<IHoneyComb> target, IEntity<IProps> settings) : base(
            () =>
            {
                // unique initial zones
                var zones = new List<IZone>(
                    new Mapped<double, IZone>(
                        speed => new Around(speed, settings),
                        new TargetSpeeds(target)
                    )
                );

                if(zones.Count == 0)
                {
                    zones.Add(new Around(FallbackSpeed, settings));
                }

                // number of overlaps
                var overlaps = 0;
                for (var i = 0; i < zones.Count - 1; i++)
                {
                    if (zones[i].Max() > zones[i + 1].Min())
                    {
                        overlaps++;
                    }
                }

                // merge the two zones which are closest together
                // as long as we have too many (overlapping) zones
                // overlaps will be replaced with zones later
                while((zones.Count + overlaps) > 5)
                {
                    var min_range = double.PositiveInfinity;
                    var min_range_index = -1;
                    for (var i = 0; i < zones.Count -1; i++)
                    {
                        var r = zones[i+1].Max() - zones[i].Min();
                        if (r < min_range)
                        {
                            min_range = r;
                            min_range_index = i;
                        }
                    }
                    var new_zone = new Across(zones[min_range_index], zones[min_range_index + 1]);
                    zones.RemoveRange(min_range_index, 2);
                    zones.Insert(min_range_index, new_zone);

                    // overlaps might have changed
                    overlaps = 0;
                    for (var i = 0; i < zones.Count - 1; i++)
                    {
                        if (zones[i].Max() > zones[i + 1].Min())
                        {
                            overlaps++;
                        }
                    }
                }

                // create new zones from overlaps
                while (overlaps > 0)
                {
                    var max_overlap = 0.0;
                    var max_overlap_index = -1;
                    for(var i = 0;i < zones.Count - 1; i++)
                    {
                        var overlap = zones[i].Max() - zones[i + 1].Min();
                        if (overlap > max_overlap)
                        {
                            max_overlap = overlap;
                            max_overlap_index = i;
                        }
                    }

                    var new_zone = new InsideOverlap(zones[max_overlap_index], zones[max_overlap_index + 1]);
                    var below = new Retreated(zones[max_overlap_index], new_zone);
                    var above = new Retreated(zones[max_overlap_index + 1], new_zone);
                    zones.RemoveRange(max_overlap_index, 2);
                    zones.InsertRange(max_overlap_index, new ListOf<IZone>(below, new_zone, above));
                    overlaps--;
                }

                // if we ended up (or started) with less than 5 zones,
                // try to shift some in gaps
                var gap_fillers = new List<IZone>();
                for (var i = 0; i < zones.Count - 1; i++)
                {
                    if ((zones[i+1].Min() - zones[i].Max()) > new ZoneRadius.Of(settings).Value())
                    {
                        gap_fillers.Add(new Between(zones[i], zones[i+1]));
                    }
                }
                gap_fillers.Sort((zone1, zone2) => (zone1.Max() - zone1.Min()).CompareTo(zone2.Max() - zone2.Min()));

                while(zones.Count < 5 && gap_fillers.Count > 0)
                {
                    zones.Add(gap_fillers[^1]);
                    gap_fillers.RemoveAt(gap_fillers.Count - 1);
                }
                zones.Sort((zone1, zone2) => zone1.Min().CompareTo(zone2.Min()));

                // if we still have less than 5 zones,
                // just put them on top
                while(zones.Count < 5)
                {
                    zones.Add(new Beyond(zones[^1], settings));
                }

                // finally, "grow" the zones to make them adjacent
                for (var i = 0; i < zones.Count - 1; i++)
                {
                    if(zones[i].Max() != zones[i + 1].Min())
                    {
                        var center = (zones[i].Max() + zones[i + 1].Min()) / 2;
                        var new_below = new Advanced(zones[i], center);
                        var new_above = new Advanced(zones[i+1], center);
                        zones.RemoveRange(i, 2);
                        zones.InsertRange(i, new ListOf<IZone>(new_below, new_above)
                        );
                    }
                }

                return zones;
            },
            false

        )
        { }
    }
}

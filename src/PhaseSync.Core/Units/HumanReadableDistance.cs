using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings.Input;
using System.Globalization;
using Xive;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Units
{
    public sealed class HumanReadableDistance : TextEnvelope
    {
        public HumanReadableDistance(double distanceInM, IEntity<IProps> settings) : base(() =>
            new FallbackMap<string, string>(
                MapOf.New(
                    KvpOf.New("IMPERIAL", () => (distanceInM / 1609.34).ToString("F1", CultureInfo.InvariantCulture) + " mi")
                ),
                unknown => (distanceInM / 1000).ToString("F1", CultureInfo.InvariantCulture) + "km"
            )[new ZoneUnit.Has(settings).Value() ? new ZoneUnit.Of(settings).Value() : "METRIC"],
            false
        )
        { }
    }
}

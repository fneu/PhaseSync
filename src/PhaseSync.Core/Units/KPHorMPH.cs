using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings.Input;
using System.Globalization;
using Xive;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Units
{
    public sealed class KPHorMPH : TextEnvelope
    {
        public KPHorMPH(double speedInMPS, IEntity<IProps> settings) : base(() =>
            new FallbackMap<string, string>(
                MapOf.New(
                    KvpOf.New("IMPERIAL", () => (speedInMPS * 2.23694).ToString("F1", CultureInfo.InvariantCulture))
                ),
                unknown => (speedInMPS * 3.6).ToString("F1", CultureInfo.InvariantCulture)
            )[new ZoneUnit.Has(settings).Value() ? new ZoneUnit.Of(settings).Value() : "METRIC"],
            false
        )
        { }
    }
}

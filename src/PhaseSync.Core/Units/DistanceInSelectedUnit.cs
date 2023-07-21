using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings.Input;
using Xive;
using Yaapii.Atoms.Map;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Units
{
    public sealed class DistanceInSelectedUnit : ScalarEnvelope<double>
    {
        public DistanceInSelectedUnit(double distanceInM, IEntity<IProps> settings) : base(() =>
            new FallbackMap<string, double>(
                MapOf.New(
                    KvpOf.New("IMPERIAL", () => (distanceInM / 1609.34))
                ),
                unknown => (distanceInM / 1000)
            )[new ZoneUnit.Has(settings).Value() ? new ZoneUnit.Of(settings).Value() : "METRIC"]
        )
        { }
    }
}

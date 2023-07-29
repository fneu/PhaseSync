using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Zones
{
    public abstract class ZoneEnvelope : IZone
    {
        private readonly IScalar<IZone> zone;

        public ZoneEnvelope(IZone zone) : this(() => zone)
        { }

        public ZoneEnvelope(Func<IZone> zone) :this(new ScalarOf<IZone>(zone))
        { }

        public ZoneEnvelope(IScalar<IZone> zone)
        {
            this.zone = zone;
        }

        public double Min()
        {
            return this.zone.Value().Min();
        }

        public double Max()
        {
            return this.zone.Value().Max();
        }
    }
}

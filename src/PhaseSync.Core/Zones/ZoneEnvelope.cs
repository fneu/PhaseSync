namespace PhaseSync.Core.Zones
{
    public abstract class ZoneEnvelope : IZone
    {
        private readonly Func<IZone> zone;

        public ZoneEnvelope(IZone zone) : this(() => zone)
        { }

        public ZoneEnvelope(Func<IZone> zone)
        {
            this.zone = zone;
        }

        public double Min()
        {
            return this.zone.Invoke().Min();
        }

        public double Max()
        {
            return this.zone.Invoke().Max();
        }
    }
}

namespace PhaseSync.Core.Zones
{
    public sealed class Advanced : ZoneEnvelope
    {
        public Advanced(IZone old, double speed) : base(
            () =>
            {
                var center = (old.Min() + old.Max()) / 2;
                if (speed <= center)
                {
                    return new ZoneOf(speed, old.Max());
                }
                else
                {
                    return new ZoneOf(old.Min(), speed);
                }
            }
        )
        { }
    }
}

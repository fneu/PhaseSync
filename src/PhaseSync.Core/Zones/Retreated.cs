namespace PhaseSync.Core.Zones
{
    public sealed class Retreated : ZoneEnvelope
    {
        public Retreated(IZone old, IZone other) : base(
            () =>
            {
                if (old.Min() < other.Min())
                {
                    return new ZoneOf(old.Min(), other.Min());
                }
                else if (old.Max() > other.Max())
                {

                    return new ZoneOf(other.Max(), old.Max());
                }
                else
                {
                    throw new ArgumentException("Cannot retreat Zone, other encloses it!");
                }
            }
        )
        { }
    }
}

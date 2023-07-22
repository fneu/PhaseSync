namespace PhaseSync.Core.Zones
{
    public sealed class InsideOverlap : ZoneEnvelope
    {
        public InsideOverlap(IZone below, IZone above) : base(
            new ZoneOf(above.Min(), below.Max())
        )
        { }
    }
}

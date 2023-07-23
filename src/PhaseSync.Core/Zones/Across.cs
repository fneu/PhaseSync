namespace PhaseSync.Core.Zones
{
    public sealed class Across : ZoneEnvelope
    {
        public Across( IZone below, IZone above) : base(
            new ZoneOf(below.Min(), above.Max())
            )
        { }
    }
}

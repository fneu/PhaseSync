namespace PhaseSync.Core.Zones
{
    public sealed class Between : ZoneEnvelope
    {
        public Between(IZone below, IZone above) : base(
            new ZoneOf(below.Max(), above.Min())
        )
        { }
    }
}

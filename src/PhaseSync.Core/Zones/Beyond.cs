using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings.Input;
using Xive;

namespace PhaseSync.Core.Zones
{
    public sealed class Beyond : ZoneEnvelope
    {
        public Beyond(IZone below, IEntity<IProps> settings) : base(
            () =>
            {
                var radius = new ZoneRadius.Of(settings).Value();
                return new ZoneOf(
                    below.Max(),
                    below.Max() + 2 * radius
                );
            }
        )
        { }
    }
}

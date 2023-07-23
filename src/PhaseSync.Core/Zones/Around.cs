using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings.Input;
using Xive;

namespace PhaseSync.Core.Zones
{
    public sealed class Around : ZoneEnvelope
    {
        public Around(double speed, IEntity<IProps> settings) : base(
            () =>
            {
                var radius = new ZoneRadius.Of(settings).Value();
                return
                    new ZoneOf(
                        speed - radius,
                        speed + radius
                    );
            }
        )
        { }
    }
}

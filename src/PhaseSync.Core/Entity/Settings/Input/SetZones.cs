using Xive;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.Settings.Input
{
    public sealed class SetZones : EntityInputEnvelope<IProps>
    {
        private const string KEY = "zone.set";

        public SetZones(bool value) : base(
            (props) => props.Refined(KEY, value.ToString())
        )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IProps> settings) : base(
               () => settings.Memory().Names().Contains(KEY)
            )
            { }
        }

        public sealed class Of : ScalarEnvelope<bool>
        {
            public Of(IEntity<IProps> settings) : base(
                () => new BoolOf(settings.Memory().Value(KEY)).Value()
            )
            { }
        }
    }
}

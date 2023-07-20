using Xive;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.Settings.Input
{
    public sealed class EnableSync : EntityInputEnvelope<IProps>
    {
        private const string KEY = "zone.set";

        public EnableSync(bool value) : base(
            (props) => props.Refined(KEY, value.ToString())
        )
        { }

        public sealed class Of : ScalarEnvelope<bool>
        {
            public Of(IEntity<IProps> settings) : base(
                () => settings.Memory().Names().Contains(KEY) && new BoolOf(settings.Memory().Value(KEY)).Value()
            )
            { }
        }
    }
}

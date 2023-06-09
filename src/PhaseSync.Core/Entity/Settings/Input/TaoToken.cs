using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.Settings.Input
{
    public sealed class TaoToken : EntityInputEnvelope<IProps>
    {
        private const string KEY = "tao.token";

        public TaoToken(string value) : base(
            (props) => props.Refined(KEY, value))
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IProps> settings) : base(
               () => settings.Memory().Names().Contains(KEY)
            )
            { }
        }

        public sealed class Of : ScalarEnvelope<string>
        {
            public Of(IEntity<IProps> settings) : base(
                () => settings.Memory().Value(KEY)
            )
            { }
        }
    }
}

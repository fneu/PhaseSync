using Xive;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.Settings.Input
{
    /// <summary>
    /// Zone radius in MPS
    /// </summary>
    public sealed class ZoneMas : EntityInputEnvelope<IProps>
    {
        private const string KEY = "zone.mas";

        /// <summary>
        /// Zone radius in MPS
        /// </summary>
        public ZoneMas(double value) : base(
            (props) => props.Refined(KEY, new TextOf(value).AsString()))
        { }

        /// <summary>
        /// Zone radius in MPS
        /// </summary>
        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IProps> settings) : base(
               () => settings.Memory().Names().Contains(KEY)
            )
            { }
        }

        /// <summary>
        /// Zone radius in MPS
        /// </summary>
        public sealed class Of : ScalarEnvelope<double>
        {
            public Of(IEntity<IProps> settings) : base(
                () => new DoubleOf(settings.Memory().Value(KEY)).Value()
            )
            { }
        }
    }
}

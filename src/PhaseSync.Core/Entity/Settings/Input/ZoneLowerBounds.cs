using Xive;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Number;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity.Settings.Input
{
    /// <summary>
    /// Lower bounds of calculated speed zones in MPS
    /// </summary>
    public sealed class ZoneLowerBounds : EntityInputEnvelope<IProps>
    {
        private const string KEY = "zone.lower-bounds";

        /// <summary>
        /// Lower bounds of calculated speed zones in MPS
        /// </summary>
        public ZoneLowerBounds(double[] bounds) : base(
            (props) => 
                props.Refined(
                    KEY, 
                    new Yaapii.Atoms.Enumerable.Mapped<double, string>(
                        b => new TextOf(b).AsString(),
                        bounds).ToArray()
                )
        )
        { }

        /// <summary>
        /// Lower bounds of calculated speed zones in MPS
        /// </summary>
        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IProps> settings) : base(
               () => settings.Memory().Names().Contains(KEY)
            )
            { }
        }

        /// <summary>
        /// Lower bounds of calculated speed zones in MPS
        /// </summary>
        public sealed class Of : ListEnvelope<double>
        {
            public Of(IEntity<IProps> settings) : base(
                () => new ListOf<double>(
                    new Sorted<double>(
                        new Yaapii.Atoms.Enumerable.Mapped<string, double>(
                            s => new NumberOf(s).AsDouble(),
                            settings.Memory().Values(KEY)
                        )
                    )
                ),
                false
            )
            { }
        }
    }
}

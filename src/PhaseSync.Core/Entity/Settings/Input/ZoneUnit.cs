using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.Settings.Input
{
    public sealed class ZoneUnit : EntityInputEnvelope<IProps>
    {
        private const string KEY = "zone.unit";

        public ZoneUnit(string value) : base(
            (props) => props.Refined(KEY, new Valid(value).Value())
        )
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
                () => new Valid(settings.Memory().Value(KEY)).Value()
            )
            { }
        }

        private sealed class Valid : ScalarEnvelope<string>
        {
            public Valid(string value) : base(
                () => new string[] { "METRIC", "IMPERIAL" }.Contains(value) ? value : "METRIC"
            )
            { }
        }
    }
}

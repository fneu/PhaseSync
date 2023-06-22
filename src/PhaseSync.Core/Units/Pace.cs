using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using Xive;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Units
{
    public sealed class Pace : TextEnvelope
    {
        public Pace(double velocityMPS, IHive userHive) : this(
            velocityMPS, new SettingsOf(userHive)
            )
        { }

        public Pace(double velocityMPS, IEntity<IProps> settings) : this(
            velocityMPS,
            new ZoneUnit.Has(settings).Value() && new ZoneUnit.Of(settings).Value() == "METRIC"
        )
        { }

        public Pace(double velocityMPS, bool metric) : base(
            () =>
            {
                if (metric)
                {
                    return $"{(int)(1000.0 / velocityMPS / 60)}:{(int)(1000.0 / velocityMPS % 60 + 0.5):D2}";
                }
                else
                {
                    return $"{(int)(1609.34 / velocityMPS / 60)}:{(int)(1609.34 / velocityMPS % 60 + 0.5):D2}";
                }
            },
            false)
        { }
    }
}

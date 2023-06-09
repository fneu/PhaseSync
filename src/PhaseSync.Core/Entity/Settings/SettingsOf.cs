using Xive;
using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.Settings
{
    public sealed class SettingsOf : EntityEnvelope<IProps>
    {
        public SettingsOf(IHive hive) : this(
            new ScalarOf<IProps>(
                hive.Comb("settings").Props()
            )
        )
        { }

        public SettingsOf(IProps props) : this(
            new ScalarOf<IProps>(props)
        )
        { }

        public SettingsOf(IScalar<IProps> props) : base(
            new EntityOf<IProps>(
                "settings",
                props.Value()
            )
        )
        { }
    }
}

using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Units
{
    public sealed class HumanReadableDuration : TextEnvelope
    {
        public HumanReadableDuration(int seconds) : base(() =>
            {
                var t = TimeSpan.FromSeconds(seconds);
                return t.Hours > 0
                    ? string.Format("{0}h {1}min", t.Hours, t.Minutes)
                    : string.Format("{0}min", t.Minutes);
            },
            false)
        {

        }
    }
}

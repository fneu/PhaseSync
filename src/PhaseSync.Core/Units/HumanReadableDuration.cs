using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Units
{
    public sealed class HumanReadableDuration : TextEnvelope
    {
        public HumanReadableDuration(int seconds) : base(() =>
            {
                var t = TimeSpan.FromSeconds(seconds);
                if (t.Hours > 0)
                {
                    return string.Format("{0}h {1}min", t.Hours, t.Minutes);
                }
                else if (t.Minutes == 0)
                {
                    return string.Format("{0}s", t.Seconds);
                }
                else {
                    if (t.Seconds == 0)
                    {
                        return string.Format("{0}min", t.Minutes);
                    }
                    else
                    {
                        return string.Format("{0}min {1}s", t.Minutes, t.Seconds);
                    }
                }
            },
            false
        )
        { }
    }
}

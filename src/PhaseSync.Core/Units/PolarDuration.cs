using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Units
{
    public sealed class PolarDuration : TextEnvelope
    {
        public PolarDuration(int seconds) : base(() =>
            {
                var t = TimeSpan.FromSeconds(seconds);
                return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
            },
            false)
        {

        }
    }
}

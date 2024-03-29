﻿using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Units
{
    public sealed class HumanReadableDuration : TextEnvelope
    {
        public HumanReadableDuration(int seconds) : base(() =>
            {
                var t = TimeSpan.FromSeconds(seconds);
                if (t.Hours > 0)
                {
                    return string.Format("{0} hours {1} minutes", t.Hours, t.Minutes);
                }
                else if (t.Minutes == 0)
                {
                    return string.Format("{0} seconds", t.Seconds);
                }
                else {
                    if (t.Seconds == 0)
                    {
                        return string.Format("{0} minutes", t.Minutes);
                    }
                    else
                    {
                        return string.Format("{0} minutes {1} seconds", t.Minutes, t.Seconds);
                    }
                }
            },
            false
        )
        { }
    }
}

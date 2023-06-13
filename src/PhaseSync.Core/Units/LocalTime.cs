using System.Globalization;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Units
{
    /// <summary>
    /// Converts a UTC time like '2023-06-25T07:00:00Z'
    /// to user local time according to the user's time zone.
    /// </summary>    
    public sealed class LocalTime : ScalarEnvelope<string>
    {
        public LocalTime(string utcTime, string timeZone) : base(
            () => 
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.ParseExact(utcTime,
                    "yyyy-MM-ddTHH:mm:ssZ",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
                ),
            TimeZoneInfo.FindSystemTimeZoneById(timeZone)
            ).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture)
        )
        { }
    }
}

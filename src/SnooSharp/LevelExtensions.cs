using System.Globalization;

namespace SnooSharp;

public static class LevelExtensions
{
    public static DateTime Start(this Level level) =>
        DateTime.ParseExact(level.startTime, Constants.Formats.LevelDateTimeFormat, CultureInfo.InvariantCulture);
    public static DateTime End(this Level level) => level.Start().AddSeconds(level.stateDuration);
}
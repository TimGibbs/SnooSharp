namespace SnooSharp;

internal static class GeneralExtensions
{
    public static IEnumerable<DateTime> EachDayUntil(this DateTime start, DateTime end)
    {
        var iDate = new DateTime(start.Year, start.Month, start.Day);
        while (iDate <= end)
        {
            yield return iDate;
            iDate = iDate.AddDays(1);
        }
    }
}
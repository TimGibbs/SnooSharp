namespace SnooSharp.Example;

public static class DataResponsesToSleepSessionMapper
{
    public static IEnumerable<SleepSession> Map(IEnumerable<DataResponse> responses)
    {
        return responses.SelectMany(o => o.levels)
            .GroupBy(o=>o.sessionId)
            .Select(o=>new SleepSession(o.ToArray()))
            .Where(o=> o.StartTime != default)
            .ToArray();
    }
}
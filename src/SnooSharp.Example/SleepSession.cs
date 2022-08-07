namespace SnooSharp.Example;

public class SleepSession
{
    public SleepSession(IEnumerable<Level> levels)
    {
        var enumerable = levels as Level[] ?? levels.ToArray();
        var ids = enumerable.Select(o => o.sessionId).Distinct();
        if (!ids.IsSingle(out var id)) 
            throw new InvalidOperationException("SessionId mismatch");   
        SessionId = id;
        var asleep = enumerable.Where(o => o.type == SleepType.Asleep).ToArray();

        if (!asleep.Any())
        {
            StartTime = default;
            EndTime = default;  
        }
        
        StartTime = asleep.Min(o=>o.Start());
        EndTime = asleep.Max(o => o.End());
    }
    
    public string? SessionId { get; }
    //public SleepType type { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
}
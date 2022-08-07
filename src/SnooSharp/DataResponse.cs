namespace SnooSharp;

public class DataResponse
{
    public List<Level> levels { get; set; }
    public int naps { get; set; }
    public int longestSleep { get; set; }
    public int totalSleep { get; set; }
    public int daySleep { get; set; }
    public int nightSleep { get; set; }
    public int nightWakings { get; set; }
}
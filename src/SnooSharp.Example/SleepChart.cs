using System.Text.RegularExpressions;
using Spectre.Console;

namespace SnooSharp.Example;

public static class SleepChart
{
    public static void MakeChart(IEnumerable<Level> levels)
    {
        var g = levels.GroupBy(o => o.Start().Date).ToDictionary(k => k.Key, ChartLine);
        var ordered = g.OrderBy(o => o.Key);
        foreach (var (key, value) in ordered)
        {
            var chart = string.Join("", value);
            chart = Regex.Replace(chart, "[1]+", m => $@"[green]{m.Value}[/]");
            chart = Regex.Replace(chart, "[2]+", m => $@"[red]{m.Value}[/]");
            chart = Regex.Replace(chart, "[012]","█");
            AnsiConsole.MarkupLine($"{key:yy-MM-dd}:{chart}");
        }
    }

    private static int[] ChartLine(IEnumerable<Level> levels)
    {
        if (!levels.Select(o => o.Start().Date).Distinct().IsSingle(out var date))
            throw new ArgumentOutOfRangeException(nameof(levels));

        var ret = new int[48];
        foreach (var level in levels)
        {
            var d = level.Start();
            d = d.AddMinutes(d.Minute % 30);
            while (d < level.End())
            {
                var pos = d.Hour * 2 + d.Minute / 30;
                if (level.type == SleepType.Asleep) ret[pos] = 1;
                if (level.type == SleepType.Soothing) ret[pos] = 2;
                d = d.AddMinutes(30);
            }
        }

        return ret;
    }
}
using SnooSharp;
using SnooSharp.Example;
using Spectre.Console;


AnsiConsole.Write(
    new FigletText("SnooSharp")
        .Centered()
        .Color(Color.Green));

var options = await OptionsRetrieval.GetOptions();

DateTime st;
DateTime et;
if (AnsiConsole.Confirm("Get previous 24 hours?"))
{
    et = DateTime.Now;
    st = et.AddDays(-1);
}
else
{
    st = DateTimeInput.GetDateTime("Start");
    et = DateTimeInput.GetDateTime("End");
}




var h = new HttpClient();
var client = new SnooClient(h, options);
var g = await client.GetDataResponses(st, et);
var j = DataResponsesToSleepSessionMapper.Map(g).Where(o=>o.EndTime>st && o.StartTime<et);
var k = j.OrderBy(o => o.StartTime);
foreach (var sleepSession in k)
{
    AnsiConsole.MarkupLine($"[orange1]{sleepSession.StartTime:yyyy-MM-dd HH:mm}[/]->[orange1]{sleepSession.EndTime:yyyy-MM-dd HH:mm}[/]");
}

SleepChart.MakeChart(g.SelectMany(o=>o.levels ?? Array.Empty<Level>().AsEnumerable()));

AnsiConsole.MarkupLine("[green]Press any key to exit...[/]");
Console.ReadKey();
    
    





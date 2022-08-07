using Spectre.Console;

namespace SnooSharp.Example;

public static class DateTimeInput
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string TimeFormat = "HH:mm";
    
    public static DateTime GetDateTime(string prompt)
    {
        DateOnly date;
        var dateOk = false;
        while (!dateOk)
        {
            var datestr = AnsiConsole.Ask<string>($"[green]{prompt} Date ({DateFormat})[/]:");
            if (DateOnly.TryParseExact(datestr, DateFormat, out date))
            {
                dateOk = true;
            }

            if (!dateOk)
            {
                AnsiConsole.MarkupLine("[red]Something went wrong there, please try again[/]");
            }
        }
        
        TimeOnly time;
        var timeOk = false;
        while (!timeOk)
        {
            var timestr = AnsiConsole.Ask<string>($"[green]{prompt} Time ({TimeFormat})[/]:");
            if (TimeOnly.TryParseExact(timestr, TimeFormat, out time))
            {
                timeOk = true;
            }

            if (!timeOk)
            {
                AnsiConsole.MarkupLine("[red]Something went wrong there, please try again[/]");
            }
        }

        return date.ToDateTime(time);
    }
}
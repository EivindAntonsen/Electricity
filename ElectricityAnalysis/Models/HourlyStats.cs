using System.Globalization;
using System.Text;

namespace ElectricityAnalysis.Models;

public record HourlyStats(
    DateTime Date,
    bool IsWeekend,
    string Season,
    decimal KwhConsumption,
    decimal NokPerKwh
)
{
    private const char Delimiter = ';';

    public string ToCsvRow() => new StringBuilder()
        .Append(Date.ToString("dd.MM.yyyy")).Append(Delimiter)
        .Append(Date.TimeOfDay).Append(Delimiter)
        .Append(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Date.DayOfWeek)).Append(Delimiter)
        .Append(IsWeekend ? "Weekend" : "Workday").Append(Delimiter)
        .Append(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Date.Month)).Append(Delimiter)
        .Append(Season).Append(Delimiter)
        .Append($"{KwhConsumption:N}").Append(Delimiter)
        .Append($"{NokPerKwh:N}")
        .ToString();
};
using System.Globalization;
using System.Text;

namespace ElectricityAnalysis.Models;

public record HourlyStats(
    DateTime Date,
    string Weekday,
    bool IsWeekend,
    string Month,
    string Season,
    decimal KwhConsumption,
    decimal NokPerKwh,
    decimal ExchangeRate
)
{
    private const char Delimiter = ';';

    public string ToCsvRow() => new StringBuilder()
        .Append(Date.ToString("dd.MM.yyyy")).Append(Delimiter)
        .Append(Date.TimeOfDay).Append(Delimiter)
        .Append(Date.Day).Append(Delimiter)
        .Append(Weekday).Append(Delimiter)
        .Append(IsWeekend ? "Weekend" : "Workday").Append(Delimiter)
        .Append(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Date.Month)).Append(Delimiter)
        .Append(Season).Append(Delimiter)
        .Append(KwhConsumption).Append(Delimiter)
        .Append(NokPerKwh).Append(Delimiter)
        .Append(ExchangeRate)
        .ToString();
};
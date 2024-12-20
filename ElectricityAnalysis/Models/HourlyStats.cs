using System.Text;

namespace ElectricityAnalysis.Models;

public record HourlyStats(
    int Hour,
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
        .Append($"{Hour:00}").Append(Delimiter)
        .Append($"{Date:dd.MM.yyyy}").Append(Delimiter)
        .Append(Weekday).Append(Delimiter)
        .Append(IsWeekend).Append(Delimiter)
        .Append(Month).Append(Delimiter)
        .Append(Season).Append(Delimiter)
        .Append(KwhConsumption).Append(Delimiter)
        .Append(NokPerKwh).Append(Delimiter)
        .Append(ExchangeRate).AppendLine()
        .ToString();
};
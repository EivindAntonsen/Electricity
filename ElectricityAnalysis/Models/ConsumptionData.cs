using System.Globalization;
using System.Text;
using ElectricityAnalysis.Csv;

namespace ElectricityAnalysis.Models;

public record ConsumptionData(
    TimeFrameType TimeFrameType,
    DateTime TimeStart,
    DateTime TimeEnd,
    decimal KwhConsumptionTotal,
    decimal? KwhConsumptionAverage,
    decimal? KwhConsumptionMedian,
    decimal NokPerKwh
) : ITimeFrame, ICsvWritable
{
    private const char Delimiter = ';';


    public string ToCsvRow() => new StringBuilder()
        .Append(TimeStart.ToString("dd.MM.yyyy"))
        .Append(Delimiter)
        .Append(TimeStart.TimeOfDay).Append(Delimiter)
        .Append(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(TimeStart.DayOfWeek))
        .Append(Delimiter)
        .Append(TimeFrameType is TimeFrameType.Weekend ? "Weekend" : "Workday")
        .Append(Delimiter)
        .Append(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(TimeStart.Month))
        .Append(Delimiter)
        .Append(TimeStart.Month.ToSeason())
        .Append(Delimiter)
        .Append($"{KwhConsumptionTotal:N}")
        .Append(Delimiter)
        .Append($"{KwhConsumptionAverage:N}")
        .Append(Delimiter)
        .Append($"{KwhConsumptionMedian:N}")
        .Append(Delimiter)
        .Append($"{NokPerKwh:N}")
        .ToString();
};
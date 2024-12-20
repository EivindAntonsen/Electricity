using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Csv;

public interface ICsvWriter
{
    Task WriteHourlyPriceDataAsync(IEnumerable<HourlyPriceData> priceDatas);
    Task WriteHourlyStatsAsync(IEnumerable<HourlyStats> stats);
}
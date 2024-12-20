using ElectricityAnalysis.Analysis;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Data;

public interface ICsvWriter
{
    Task WriteHourlyPriceDataAsync(IEnumerable<HourlyPriceData> priceDatas);
    Task WriteHourlyStatsAsync(IEnumerable<HourlyStats> stats);
}
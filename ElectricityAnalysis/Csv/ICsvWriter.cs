using ElectricityAnalysis.Integrations.Price;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Csv;

public interface ICsvWriter
{
    Task WriteHourlyPriceDataAsync(IEnumerable<PricePoint> priceDatas);
    Task WritePeriodicStatsAsync(IEnumerable<ConsumptionData> stats);
}
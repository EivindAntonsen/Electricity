using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Analysis;

public interface IElectricityAnalyzer
{
    IEnumerable<HourlyStats> CalculateHourlyStats(
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<HourlyPriceData> hourlyPriceDatas
    );
}
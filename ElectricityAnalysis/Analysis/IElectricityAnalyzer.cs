using ElectricityAnalysis.Integrations.Price;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Analysis;

public interface IElectricityAnalyzer
{
    IEnumerable<ConsumptionData> CalculateHourlyStats(
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<PricePoint> hourlyPriceDatas
    );

    public IEnumerable<PeriodicPricePoints> GetPeriodicPricePointsForTimeFrames(
        TimeFrameType timeFrameType,
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<PricePoint> hourlyPriceDatas
    );
}
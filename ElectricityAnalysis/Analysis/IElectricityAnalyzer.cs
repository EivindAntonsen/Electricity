using ElectricityAnalysis.Integrations.Price;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Analysis;

public interface IElectricityAnalyzer
{
    IEnumerable<ConsumptionData> CalculateConsumptionData(
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<PricePoint> hourlyPriceDatas
    );

    public IEnumerable<PeriodicPricePoints> GetPeriodicPricePointsPerTimeFrame(
        TimeFrameType timeFrameType,
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<PricePoint> hourlyPriceDatas
    );
}
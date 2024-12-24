using ElectricityAnalysis.Integrations.Price;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Csv;

public interface ICsvReader
{
    IEnumerable<MeteringValue> GetMeteringValues();
    Task<IEnumerable<PricePoint>> GetHourlyPriceDataAsync(CancellationToken cancellationToken);
}
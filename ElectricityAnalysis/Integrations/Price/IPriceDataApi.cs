using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Integrations.Price;

public interface IPriceDataApi
{
    Task<IEnumerable<PricePoint>> GetElectricityPriceAsync(
        DateOnly date,
        PriceArea area,
        CancellationToken cancellationToken);
}
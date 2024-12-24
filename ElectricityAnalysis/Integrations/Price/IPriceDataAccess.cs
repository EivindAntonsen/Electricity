using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Integrations.Price;

public interface IPriceDataAccess
{
    Task<IEnumerable<PricePoint>> GetHourlyElectricityPrices(
        PriceArea area, 
        CancellationToken cancellationToken = default);
}
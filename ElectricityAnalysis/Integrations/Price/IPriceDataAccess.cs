using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Integrations.Price;

public interface IPriceDataAccess
{
    Task<IEnumerable<HourlyPriceData>> GetHourlyElectricityPrices(
        ElectricityPriceArea area, 
        CancellationToken cancellationToken = default);
}
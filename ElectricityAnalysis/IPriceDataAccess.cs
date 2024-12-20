using ElectricityAnalysis.Models;

namespace ElectricityAnalysis;

public interface IPriceDataAccess
{
    Task<IEnumerable<HourlyPriceData>> GetHourlyElectricityPrices(
        ElectricityPriceArea area, 
        CancellationToken cancellationToken = default);
}
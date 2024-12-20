using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Integrations;

public interface IBeneficialAppsIntegration
{
    Task<List<HourlyPriceData>> GetAllHourlyElectricityPricesAsync(
        ElectricityPriceArea area,
        CancellationToken cancellationToken);

    Task<IEnumerable<HourlyPriceData>> GetElectricityPriceAsync(
        int year,
        int month,
        int day,
        ElectricityPriceArea area,
        CancellationToken cancellationToken);
}
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Integrations.Price;

public interface IBeneficialAppsIntegration
{
    Task<IEnumerable<HourlyPriceData>> GetElectricityPriceAsync(
        int year,
        int month,
        int day,
        ElectricityPriceArea area,
        CancellationToken cancellationToken);
}
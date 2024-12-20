using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Data;

public interface ICsvReader
{
    IEnumerable<MeteringValue> GetMeteringValues();
    Task<IEnumerable<HourlyPriceData>> GetHourlyPriceDataAsync(CancellationToken cancellationToken);
}
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Csv;

public interface ICsvReader
{
    IEnumerable<MeteringValue> GetMeteringValues();
    Task<IEnumerable<HourlyPriceData>> GetHourlyPriceDataAsync(CancellationToken cancellationToken);
}
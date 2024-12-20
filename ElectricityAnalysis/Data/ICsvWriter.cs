using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Data;

public interface ICsvWriter
{
    Task WriteHourlyPriceDataAsync(IEnumerable<HourlyPriceData> priceDatas);
}
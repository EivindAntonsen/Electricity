using System.Formats.Tar;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Analysis;

public class PriceAnalyzer
{
    public Dictionary<int, decimal> CalculateAveragePricesByHour(IEnumerable<HourlyPriceData> priceDatas) => priceDatas
        .GroupBy(data => data.TimeStart.Hour)
        .ToDictionary(
            data => data.Key,
            data => data.AsEnumerable())
        .Select(entry =>
            new KeyValuePair<int, decimal>(entry.Key, entry.Value.Average(value => value.NokPerKWh)))
        .ToDictionary(entry => entry.Key, entry => entry.Value);

    public Dictionary<int, decimal> CalculateAveragePowerConsumptionByHour(IEnumerable<MeteringValue> meteringValues) => meteringValues
        .GroupBy(data => data.Start.Hour)
        .ToDictionary(
            data => data.Key,
            data => data.AsEnumerable())
        .Select(entry =>
            new KeyValuePair<int, decimal>(entry.Key, entry.Value.Average(value => value.Value)))
        .ToDictionary(entry => entry.Key, entry => entry.Value);

    public void CalculateAverageHourlyPriceAndPowerConsumption(IEnumerable<HourlyPriceData> priceDatas, IEnumerable<MeteringValue> meteringValues)
    {
        var averagePricesByHour = CalculateAveragePricesByHour(priceDatas);
        var averagePowerConsumptionByHour = CalculateAveragePowerConsumptionByHour(meteringValues);
    }
}
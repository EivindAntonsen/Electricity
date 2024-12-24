using System.Globalization;
using ElectricityAnalysis.Csv;
using ElectricityAnalysis.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectricityAnalysis.Integrations.Price;

public class PriceDataProvider(
    IOptions<PriceDataConfig> config,
    IPriceDataApi priceDataApi,
    ICsvReader csvReader,
    ILogger<PriceDataProvider> logger)
    : IPriceDataAccess
{
    private readonly PriceDataConfig _config = config.Value;
    private DateTime? _previousRequest = null;

    public async Task<IEnumerable<PricePoint>> GetHourlyElectricityPrices(
        PriceArea area,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting electricity price data for {Area} from disk", area);
        var (firstDate, lastDate) = GetPriceDateRange();

        var priceDatasByOffset = (await GetPriceDataFromDiskAsync(cancellationToken))
            .GroupBy(data => (data.TimeStart - firstDate).Days)
            .ToDictionary(
                          datas => datas.Key,
                          datas => datas.AsEnumerable());

        logger.LogInformation("Found {Count} entries on disk", priceDatasByOffset.Count(entry => entry.Value.Any()));
        logger.LogDebug("Verifying price data integrity for each day");

        var hourlyPriceDatas = new List<PricePoint>();
        for (var offset = 0; offset <= (lastDate - firstDate).Days; offset++)
        {
            logger.LogDebug("Getting electricity price data for {Area} for {Date}", area, offset);
            var offsetDate = firstDate.AddDays(offset);
            if (priceDatasByOffset.TryGetValue(offset, out var priceDataFromDisk))
            {
                hourlyPriceDatas.AddRange(priceDataFromDisk);
                continue;
            }

            logger.LogInformation("Electricity price data for {Area} not found for {Date}", area, offsetDate);

            var now = DateTime.Now;
            var timeSincelast = now - _previousRequest;
            if (timeSincelast < TimeSpan.FromMilliseconds(200))
            {
                logger.LogWarning("Too many requests to Beneficial Apps API. Waiting {PollingIntervalInMilliseconds}ms", _config.PollingIntervalInMilliseconds);
                await Task.Delay(TimeSpan.FromMilliseconds(_config.PollingIntervalInMilliseconds).Subtract(timeSincelast.Value), cancellationToken);
            }
            
            var priceDataFromApi =
                await priceDataApi.GetElectricityPriceAsync(new DateOnly(offsetDate.Year, offsetDate.Month, offsetDate.Day),
                                                            area,
                                                            cancellationToken);

            _previousRequest = DateTime.Now;

            hourlyPriceDatas.AddRange(priceDataFromApi);
        }

        return hourlyPriceDatas;
    }


    private (DateTime, DateTime) GetPriceDateRange()
    {
        var firstDate = DateTime.ParseExact(_config.EarliestPriceDate, _config.DateTimeFormat, CultureInfo.CurrentCulture);
        var lastDate = DateTime.Now.Date.AddDays(-_config.DaysBehindToday);

        return (firstDate.Date, lastDate.Date);
    }

    private async Task<IEnumerable<PricePoint>> GetPriceDataFromDiskAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return (await csvReader.GetHourlyPriceDataAsync(cancellationToken))
                .OrderBy(data => data.TimeStart);
        }
        catch (FileNotFoundException)
        {
            return [];
        }
    }
}
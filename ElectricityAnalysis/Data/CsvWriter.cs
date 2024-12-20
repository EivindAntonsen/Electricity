using ElectricityAnalysis.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectricityAnalysis.Data;

public class CsvWriter(IOptions<CsvConfiguration> config, ILogger<CsvWriter> logger) : ICsvWriter
{
    private readonly CsvConfiguration _csvConfig = config.Value;

    public async Task WriteHourlyPriceDataAsync(IEnumerable<HourlyPriceData> priceDatas)
    {
        var rows = priceDatas.Select(data => data.ToCsvRow()).ToList();
        var path = Path.Combine(_csvConfig.DataDirectoryPath, _csvConfig.PriceDataFileName);
        
        logger.LogInformation("Writing {Count} rows to {Path}", rows.Count, path);
        await File.WriteAllLinesAsync(path, rows);
        logger.LogInformation("Writing done");
    }

    public async Task WriteHourlyStatsAsync()
    {
        
    }
}
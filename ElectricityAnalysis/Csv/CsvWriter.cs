using ElectricityAnalysis.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectricityAnalysis.Csv;

public class CsvWriter(
    IOptions<CsvConfiguration> config,
    ILogger<CsvWriter> logger
) : ICsvWriter
{
    private readonly CsvConfiguration _csvConfig = config.Value;

    public async Task WriteHourlyPriceDataAsync(IEnumerable<HourlyPriceData> priceDatas)
    {
        var rows = priceDatas.Select(data => data.ToCsvRow()).ToList();
        var path = Path.Combine(_csvConfig.DataDirectoryPath, _csvConfig.PriceDataFileName);

        logger.LogInformation("Writing {Count} rows to {Path}", rows.Count, path);
        
        await using var writer = new StreamWriter(path);
        foreach (var row in rows) 
            await writer.WriteLineAsync(row);
        
        logger.LogInformation("Writing done");
    }

    public async Task WriteHourlyStatsAsync(IEnumerable<HourlyStats> stats)
    {
        var rows = stats.Select(stat => stat.ToCsvRow()).ToList();
        var path = Path.Combine(_csvConfig.DataDirectoryPath, _csvConfig.HourlyStatsFileName);
        
        logger.LogInformation("Writing {Count} rows to {Path}", rows.Count, path);

        await using var writer = new StreamWriter(path);
        foreach (var row in rows) 
            await writer.WriteLineAsync(row);
        
        logger.LogInformation("Writing done");
    }
}
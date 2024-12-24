using System.Globalization;
using ElectricityAnalysis.Integrations.Price;
using ElectricityAnalysis.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectricityAnalysis.Csv;

public class CsvReader(
    IOptions<CsvConfiguration> config,
    ILogger<CsvReader> logger
) : ICsvReader
{
    private readonly CsvConfiguration _config = config.Value;

    public IEnumerable<MeteringValue> GetMeteringValues() =>
        from directories in Directory.GetDirectories(_config.DataDirectoryPath)
        from files in Directory.GetFiles(directories, "*.csv")
        from lines in File.ReadAllLines(files).Skip(1)
        let columns = lines.Split(_config.Delimiter)
        let start = DateTime.ParseExact(columns[0], _config.DateFormat, CultureInfo.InvariantCulture)
        let end = DateTime.ParseExact(columns[1], _config.DateFormat, CultureInfo.InvariantCulture)
        let value = decimal.Parse(columns[2])
        let success = columns[3] == _config.SuccessfulReadValue
        select new MeteringValue(start, end, value, success);


    public async Task<IEnumerable<PricePoint>> GetHourlyPriceDataAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_config.DataDirectoryPath))
        {
            logger.LogWarning("Data directory does not exist: {Path}", _config.DataDirectoryPath);
            return [];
        }

        var path = Path.Combine(_config.DataDirectoryPath, _config.PriceDataFileName);

        if (File.Exists(path))
        {
            return (await File.ReadAllLinesAsync(path, cancellationToken))
                .Select(lines => lines.Split(_config.Delimiter))
                .Select(columns =>
                {
                    var nokPerKwh = decimal.Parse(columns[0], CultureInfo.InvariantCulture);
                    var eurPerKwh = decimal.Parse(columns[1], CultureInfo.InvariantCulture);
                    
                    if (decimal.TryParse(columns[2], CultureInfo.InvariantCulture, out var exchangeRate))
                    {
                        var timeEnd = DateTime.ParseExact(columns[4], _config.DateFormat, CultureInfo.InvariantCulture);
                        var timeStart = DateTime.ParseExact(columns[3], _config.DateFormat, CultureInfo.InvariantCulture);

                        return new PricePoint(timeEnd, timeStart, exchangeRate, nokPerKwh, eurPerKwh);
                    }

                    logger.LogWarning("Could not parse exchange rate: {ExchangeRate}", columns[2]);
                    foreach (var column in columns)
                    {
                        logger.LogWarning("Column: {Column}", column);
                    }

                    throw new Exception("Could not parse exchange rate");
                });
        }

        logger.LogWarning("Price data file does not exist: {Path}", path);
        return [];
    }
}
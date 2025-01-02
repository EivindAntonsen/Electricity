using ElectricityAnalysis.Analysis;
using ElectricityAnalysis.Csv;
using ElectricityAnalysis.Integrations.Price;
using ElectricityAnalysis.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

const string environment = @"C:\Environment\Electricity\Environment.json";

var host = Host
    .CreateDefaultBuilder()
    .ConfigureHostConfiguration(configurationBuilder => configurationBuilder
                                    .AddJsonFile(environment, false, true)
                                    .AddJsonFile(@"C:\Git\Electricity\ElectricityAnalysis\appsettings.json", false, true))
    .ConfigureLogging((context, builder) =>
    {
        builder.ClearProviders();
        builder.AddConsole();
        builder.AddDebug();
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddConfiguration(context.Configuration);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddCsvConfiguration();
        services.AddPriceDataConfiguration(context.Configuration);
        services.AddSingleton<ICsvReader, CsvReader>();
        services.AddSingleton<ICsvWriter, CsvWriter>();
        services.AddSingleton<IPriceDataApi, PriceDataApi>();
        services.AddSingleton<IPriceDataAccess, PriceDataProvider>();
        services.AddSingleton<IElectricityAnalyzer, ElectricityAnalyzer>();
    })
    .Build();

var dataDirectoryPath = host.Services.GetRequiredService<IConfiguration>().GetValue<string>("DataDirectoryPath")!;
var csvReader = host.Services.GetRequiredService<ICsvReader>();
var csvWriter = host.Services.GetRequiredService<ICsvWriter>();
var priceDataAccess = host.Services.GetRequiredService<IPriceDataAccess>();
var electricityAnalyzer = host.Services.GetRequiredService<IElectricityAnalyzer>();

var hourlyPriceData = (await priceDataAccess
                           .GetHourlyElectricityPrices(PriceArea.Oslo))
    .ToList();

// todo - skip serialization when new content is the same as old content
await csvWriter.WriteCsvAsync(hourlyPriceData, Path.Combine(dataDirectoryPath, "hourlyPriceData.csv"));

var meteringValues = csvReader
    .GetMeteringValues()
    .OrderBy(value => value.TimeStart)
    .ToList();

var consumptionDatas = electricityAnalyzer
    .CalculateConsumptionData(meteringValues, hourlyPriceData)
    .ToList();

await csvWriter.WriteCsvAsync(consumptionDatas, Path.Combine(dataDirectoryPath, "consumptionData.csv"));

var pricesBySeasons =
    (from periodicPricePoints in electricityAnalyzer.GetPeriodicPricePointsPerTimeFrame(TimeFrameType.Season, meteringValues, hourlyPriceData)
     group periodicPricePoints by periodicPricePoints.TimeStart.Month.ToSeason()
     into newGroup
     orderby newGroup.Key, newGroup.OrderBy(p => p.TimeStart).FirstOrDefault()?.TimeStart
     select newGroup)
    .ToDictionary(grouping => grouping.Key,
                  grouping => grouping.ToList());

foreach (var (key, seasonalPrices) in pricesBySeasons)
{
    var startDate = seasonalPrices.First().TimeStart;
    var year = startDate.Year;
    var fileName = $"{key}-{year}.csv";
    var path = Path.Combine(dataDirectoryPath, fileName);
    await csvWriter.WriteCsvAsync(seasonalPrices, path);
}
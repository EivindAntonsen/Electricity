using ElectricityAnalysis;
using ElectricityAnalysis.Analysis;
using ElectricityAnalysis.Csv;
using ElectricityAnalysis.Integrations.Price;
using ElectricityAnalysis.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

const string environment = @"C:\Environment\ElectricityAnalysis\Environment.json";

var host = Host
    .CreateDefaultBuilder()
    .ConfigureHostConfiguration(configurationBuilder => configurationBuilder
        .AddJsonFile(environment, false, true)
        .AddJsonFile(@"C:\git\ElectricityAnalysis\ElectricityAnalysis\appsettings.json", false, true))
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

var csvReader = host.Services.GetRequiredService<ICsvReader>();
var csvWriter = host.Services.GetRequiredService<ICsvWriter>();
var priceDataAccess = host.Services.GetRequiredService<IPriceDataAccess>();
var electricityAnalyzer = host.Services.GetRequiredService<IElectricityAnalyzer>();

var hourlyPriceData = (await priceDataAccess
    .GetHourlyElectricityPrices(PriceArea.Oslo))
    .ToList();

// todo - skip serialization when new content is the same as old content
await csvWriter.WriteHourlyPriceDataAsync(hourlyPriceData);

var meteringValues = csvReader
    .GetMeteringValues()
    .OrderBy(value => value.TimeStart)
    .ToList();

var hourlyStats = electricityAnalyzer
    .CalculateHourlyStats(meteringValues, hourlyPriceData)
    .ToList();

await csvWriter.WritePeriodicStatsAsync(hourlyStats);
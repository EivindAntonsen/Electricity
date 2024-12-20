using ElectricityAnalysis;
using ElectricityAnalysis.Data;
using ElectricityAnalysis.Integrations;
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
        services.AddSingleton<IBeneficialAppsIntegration, BeneficialAppsIntegration>();
        services.AddSingleton<IPriceDataAccess, PriceDataAccess>();
    })
    .Build();

var csvReader = host.Services.GetRequiredService<ICsvReader>();
var csvWriter = host.Services.GetRequiredService<ICsvWriter>();
var priceDataAccess = host.Services.GetRequiredService<IPriceDataAccess>();

var usageData = csvReader
    .GetMeteringValues()
    .OrderBy(value => value.Start)
    .ToList();
var hourlyPriceDatas = await priceDataAccess
    .GetHourlyElectricityPrices(ElectricityPriceArea.Oslo);

await csvWriter.WriteHourlyPriceDataAsync(hourlyPriceDatas);
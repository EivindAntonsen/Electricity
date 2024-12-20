using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricityAnalysis.Integrations.Price;

public record PriceDataConfig
{
    public const string SectionName = "Price";
    public required int DaysBehindToday { get; init; }
    public required string EarliestPriceDate { get; init; }
    public required string DateTimeFormat { get; init; }
    public required int PollingIntervalInMilliseconds { get; init; }
};

public static class PriceDataConfigExtensions
{
    public static IServiceCollection AddPriceDataConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PriceDataConfig>()
            .BindConfiguration(PriceDataConfig.SectionName)
            .Validate(config => config.DaysBehindToday > 0,
                $"{nameof(PriceDataConfig.DaysBehindToday)} must be greater than 0")
            .Validate(config =>  !string.IsNullOrWhiteSpace(config.EarliestPriceDate),
                $"{nameof(PriceDataConfig.EarliestPriceDate)} is required")
            .Validate(config => !string.IsNullOrWhiteSpace(config.DateTimeFormat),
                $"{nameof(PriceDataConfig.DateTimeFormat)} is required")
            .Validate(config => config.PollingIntervalInMilliseconds > 0,
                $"{nameof(PriceDataConfig.PollingIntervalInMilliseconds)} must be greater than 0")
            .ValidateOnStart();

        return services;
    }
}
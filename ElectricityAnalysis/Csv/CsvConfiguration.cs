using Microsoft.Extensions.DependencyInjection;

namespace ElectricityAnalysis.Data;

public record CsvConfiguration()
{
    public const string SectionName = "Csv";
    public required string DataDirectoryPath { get; init; }
    public required char Delimiter { get; init; }
    public required string DateFormat { get; init; }
    public required string SuccessfulReadValue { get; init; }
    public required string PriceDataFileName { get; init; }
    public required string HourlyStatsFileName { get; init; }
};

public static class CsvConfigurationExtensions
{
    public static void AddCsvConfiguration(this IServiceCollection services)
    {
        services.AddOptions<CsvConfiguration>()
            .BindConfiguration(CsvConfiguration.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.DataDirectoryPath),
                $"{nameof(CsvConfiguration.DataDirectoryPath)} is required")
            .Validate(options => !string.IsNullOrWhiteSpace(options.DateFormat),
                $"{nameof(CsvConfiguration.DateFormat)} is required")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SuccessfulReadValue),
                $"{nameof(CsvConfiguration.SuccessfulReadValue)} is required")
            .ValidateOnStart();
    }
}
using System.Text.Json.Serialization;
using ElectricityAnalysis.Csv;

namespace ElectricityAnalysis.Models;

public record PricePoint(
    [property: JsonPropertyName("time_end")]
    DateTime TimeEnd,
    [property: JsonPropertyName("time_start")]
    DateTime TimeStart,
    [property: JsonPropertyName("EXR")]
    decimal ExchangeRate,
    [property: JsonPropertyName("EUR_per_kWh")]
    decimal EurPerKWh,
    [property: JsonPropertyName("NOK_per_kWh")]
    decimal NokPerKWh) : ICsvWritable
{
    private static char Delimiter => ';';

    public string ToCsvRow() =>
        $"{NokPerKWh:N}{Delimiter}" +
        $"{EurPerKWh:N}{Delimiter}" +
        $"{ExchangeRate:00.00}{Delimiter}" +
        $"{TimeStart:dd.MM.yyyy HH:mm}{Delimiter}" +
        $"{TimeEnd:dd.MM.yyyy HH:mm}";
};
using System.Text.Json.Serialization;

namespace ElectricityAnalysis.Models;

public record HourlyPriceData(
    [property: JsonPropertyName("NOK_per_kWh")]
    decimal NokPerKWh,
    [property: JsonPropertyName("EUR_per_kWh")]
    decimal EurPerKWh,
    [property: JsonPropertyName("EXR")] 
    decimal ExchangeRate,
    [property: JsonPropertyName("time_start")]
    DateTime TimeStart,
    [property: JsonPropertyName("time_end")]
    DateTime TimeEnd
)
{
    private char Delimiter { get; } = ';';

    public string ToCsvRow() =>
        $"{NokPerKWh}{Delimiter}" +
        $"{EurPerKWh}{Delimiter}" +
        $"{ExchangeRate}{Delimiter}" +
        $"{TimeStart:dd.MM.yyyy HH:mm}{Delimiter}" +
        $"{TimeEnd:dd.MM.yyyy HH:mm}";
};
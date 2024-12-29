namespace ElectricityAnalysis.Models;

public record PeriodicPricePoint
{
    public required DateTime TimeStart { get; init; }
    public required DateTime TimeEnd { get; init; }
    public required decimal NokPerKwh { get; init; }
    public required decimal ChangeFromPrevious { get; init; }
}

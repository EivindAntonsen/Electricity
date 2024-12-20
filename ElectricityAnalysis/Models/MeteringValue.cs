namespace ElectricityAnalysis.Models;

public record MeteringValue(DateTime Start, DateTime End, decimal Value, bool Success);
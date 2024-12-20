namespace ElectricityAnalysis.Models;

public record MeteringValue(DateTime TimeStart, DateTime TimeEnd, decimal Value, bool Success);
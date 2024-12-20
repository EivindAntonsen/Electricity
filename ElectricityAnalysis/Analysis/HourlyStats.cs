namespace ElectricityAnalysis.Analysis;

public record HourlyStats(
    int Hour,
    DateTime Date,
    string Weekday, 
    bool IsWeekend,
    string Month,
    string Season, 
    decimal KwhConsumption, 
    decimal NokPerKwh,
    decimal ExchangeRate
    );
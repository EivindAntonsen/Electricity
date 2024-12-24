namespace ElectricityAnalysis.Models;

public record PeriodicPricePoints : ITimeFrame, IPeriodicPricePoints
{
    public required TimeFrameType TimeFrameType { get; init; }
    public required DateTime TimeStart { get; init; }
    public required DateTime TimeEnd { get; init; }
    public required List<PricePoint> PricePoints { get; init; }

    public decimal TotalCost() =>
        PricePoints.Sum(pricePoint => pricePoint.NokPerKWh);

    public decimal NokPerKwhAverage() =>
        PricePoints.Average(pricePoint => pricePoint.NokPerKWh);

    public decimal NokPerKwhMedian()
    {
        var sortedList = PricePoints.OrderBy(pricePoint => pricePoint.NokPerKWh).ToList();
        var middleIndex = sortedList.Count / 2;

        return sortedList[middleIndex].NokPerKWh;
    }

    public decimal NokPerKwhMax() =>
        PricePoints.Max(pricePoint => pricePoint.NokPerKWh);

    public decimal NokPerKwhMin() =>
        PricePoints.Min(pricePoint => pricePoint.NokPerKWh);
}
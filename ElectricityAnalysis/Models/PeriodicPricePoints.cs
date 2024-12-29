using System.Text;

namespace ElectricityAnalysis.Models;

public record PeriodicPricePoints : ITimeFrame, IPeriodicPricePoints
{
    private const char Delimiter = ';';
    public required TimeFrameType TimeFrameType { get; init; }
    public required DateTime TimeStart { get; init; }
    public required DateTime TimeEnd { get; init; }
    public required List<PeriodicPricePoint> PricePoints { get; init; }

    public decimal TotalCost() =>
        PricePoints.Sum(pricePoint => pricePoint.NokPerKwh);

    public decimal NokPerKwhAverage() =>
        PricePoints.Average(pricePoint => pricePoint.NokPerKwh);

    public decimal NokPerKwhMedian()
    {
        var sortedList = PricePoints.OrderBy(pricePoint => pricePoint.NokPerKwh).ToList();
        var middleIndex = sortedList.Count / 2;

        return sortedList[middleIndex].NokPerKwh;
    }

    public decimal NokPerKwhMax() =>
        PricePoints.Max(pricePoint => pricePoint.NokPerKwh);

    public decimal NokPerKwhMin() =>
        PricePoints.Min(pricePoint => pricePoint.NokPerKwh);


    public string ToCsvRow()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(TimeFrameType);
        stringBuilder.Append(Delimiter);
        stringBuilder.Append(TimeStart);
        stringBuilder.Append(Delimiter);
        stringBuilder.Append(TimeEnd);
        stringBuilder.Append(Delimiter);
        stringBuilder.Append(TotalCost());
        stringBuilder.Append(Delimiter);
        stringBuilder.Append(NokPerKwhAverage());
        stringBuilder.Append(Delimiter);
        stringBuilder.Append(NokPerKwhMedian());
        stringBuilder.Append(Delimiter);
        stringBuilder.Append(NokPerKwhMin());
        stringBuilder.Append(Delimiter);
        stringBuilder.Append(NokPerKwhMax());

        return stringBuilder.ToString();
    }
}
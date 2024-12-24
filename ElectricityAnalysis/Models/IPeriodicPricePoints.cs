namespace ElectricityAnalysis.Models;

public interface IPeriodicPricePoints
{
    public List<PricePoint> PricePoints { get; init; }
    public decimal TotalCost();
    public decimal NokPerKwhAverage();
    public decimal NokPerKwhMedian();
    public decimal NokPerKwhMax();
    public decimal NokPerKwhMin();
}
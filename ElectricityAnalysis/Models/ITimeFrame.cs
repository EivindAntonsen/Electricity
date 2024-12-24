namespace ElectricityAnalysis.Models;

public interface ITimeFrame
{
    public TimeFrameType TimeFrameType { get; init; }
    public DateTime TimeStart { get; init; }
    public DateTime TimeEnd { get; init; }
}
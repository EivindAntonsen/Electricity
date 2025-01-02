using System.Text;
using ElectricityAnalysis.Csv;

namespace ElectricityAnalysis.Models;

public record PeriodicPricePoint : ICsvWritable
{
    public required DateTime TimeStart { get; init; }
    public required DateTime TimeEnd { get; init; }
    public required decimal NokPerKwh { get; init; }
    public required decimal ChangeFromPrevious { get; init; }
    public required decimal ChangeFromMinimum { get; init; }

    public string ToCsvRow()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(TimeStart.ToString("dd.MM.yyyy HH:mm"));
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(NokPerKwh);
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(ChangeFromPrevious);
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(ChangeFromMinimum);
        
        return stringBuilder.ToString();
    }
}
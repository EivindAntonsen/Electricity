using System.Globalization;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Analysis;

public class ElectricityAnalyzer : IElectricityAnalyzer
{
    public IEnumerable<ConsumptionData> CalculateHourlyStats(
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<PricePoint> hourlyPriceDatas) =>
        from meteringValue in meteringValues
        join hourlyPrice in hourlyPriceDatas
            on meteringValue.TimeStart equals hourlyPrice.TimeStart
        select new ConsumptionData(TimeFrameType.Hour,
                                   meteringValue.TimeStart,
                                   meteringValue.TimeEnd,
                                   meteringValue.Value,
                                   null,
                                   null,
                                   hourlyPrice.NokPerKWh);

    public IEnumerable<PeriodicPricePoints> GetPeriodicPricePointsForTimeFrames(
        TimeFrameType timeFrameType,
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<PricePoint> hourlyPriceDatas)
    {
        var pricePoints = hourlyPriceDatas.OrderBy(pricePoint => pricePoint.TimeStart).ToList();

        switch (timeFrameType)
        {
            case TimeFrameType.Hour:
                return
                [
                    new PeriodicPricePoints
                    {
                        TimeFrameType = TimeFrameType.Hour,
                        PricePoints = pricePoints.ToList(),
                        TimeStart = pricePoints.First().TimeStart,
                        TimeEnd = pricePoints.Last().TimeEnd,
                    }
                ];
            
            case TimeFrameType.Day:
                return pricePoints
                    .GroupBy(pricePoint => pricePoint.TimeStart.Date)
                    .Select(points => new PeriodicPricePoints
                    {
                        TimeFrameType = TimeFrameType.Day,
                        PricePoints = points.ToList(),
                        TimeStart = points.First().TimeStart,
                        TimeEnd = points.Last().TimeEnd,
                    });
                    
            case TimeFrameType.Weekend:
                return pricePoints
                    .GroupBy(pricePoint => pricePoint.TimeStart.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    .Select(points => new PeriodicPricePoints
                    {
                        TimeFrameType = TimeFrameType.Weekend,
                        PricePoints = points.ToList(),
                        TimeStart = points.First().TimeStart,
                        TimeEnd = points.Last().TimeEnd,
                    });
            default:
                
                case TimeFrameType.Week:
                return pricePoints
                    .GroupBy(pricePoint => CultureInfo
                                 .InvariantCulture
                                 .DateTimeFormat
                                 .Calendar
                                 .GetWeekOfYear(pricePoint.TimeStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    .Select(points => new PeriodicPricePoints
                    {
                        TimeFrameType = TimeFrameType.Week,
                        PricePoints = points.ToList(),
                        TimeStart = points.First().TimeStart,
                        TimeEnd = points.Last().TimeEnd,
                    });
            
            case TimeFrameType.Month:
                return pricePoints
                    .GroupBy(pricePoint => pricePoint.TimeStart.Month)
                    .Select(points => new PeriodicPricePoints
                    {
                        TimeFrameType = TimeFrameType.Month,
                        PricePoints = points.ToList(),
                        TimeStart = points.First().TimeStart,
                        TimeEnd = points.Last().TimeEnd,
                    });
            
            case TimeFrameType.Season:
                return pricePoints
                    .GroupBy(pricePoint => pricePoint.TimeStart.Month.ToSeason())
                    .Select(points => new PeriodicPricePoints
                    {
                        TimeFrameType = TimeFrameType.Season,
                        PricePoints = points.OrderBy(pricePoint => pricePoint.TimeStart).ToList(),
                        TimeStart = points.First().TimeStart,
                        TimeEnd = points.Last().TimeEnd,
                    });
            
            case TimeFrameType.Year:
                return pricePoints
                    .GroupBy(pricePoint => pricePoint.TimeStart.Year)
                    .Select(points => new PeriodicPricePoints
                    {
                        TimeFrameType = TimeFrameType.Year,
                        PricePoints = points.ToList(),
                        TimeStart = points.First().TimeStart,
                        TimeEnd = points.Last().TimeEnd,
                    });
        }
    }
}
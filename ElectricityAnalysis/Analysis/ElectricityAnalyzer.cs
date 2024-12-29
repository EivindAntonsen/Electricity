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

        return timeFrameType switch
        {
            TimeFrameType.Hour =>
                [ToHourlyPricePoints(pricePoints)],

            TimeFrameType.Day =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Date)
                    .Select(points => ToHourlyPricePoints(points.ToList())),

            TimeFrameType.Weekend =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    .Select(points => ToHourlyPricePoints(points.ToList())),

            TimeFrameType.Week =>
                pricePoints
                    .GroupBy(pricePoint => CultureInfo
                                 .InvariantCulture.DateTimeFormat.Calendar
                                 .GetWeekOfYear(pricePoint.TimeStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    .Select(points => ToHourlyPricePoints(points.ToList())),

            TimeFrameType.Month =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Month)
                    .Select(points => ToHourlyPricePoints(points.ToList())),

            TimeFrameType.Season =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Month.ToSeason())
                    .Select(points => ToHourlyPricePoints(points.ToList())),

            TimeFrameType.Year =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Year)
                    .Select(points => ToHourlyPricePoints(points.ToList())),

            _ =>
                pricePoints
                    .GroupBy(pricePoint => CultureInfo
                                 .InvariantCulture.DateTimeFormat.Calendar
                                 .GetWeekOfYear(pricePoint.TimeStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    .Select(points => ToHourlyPricePoints(points.ToList()))
        };
    }


    private static PeriodicPricePoints ToHourlyPricePoints(List<PricePoint> pricePoints)
    {
        pricePoints.Sort((a, b) => a.TimeStart.CompareTo(b.TimeStart));
        List<PeriodicPricePoint> pricePointsList = [];
        for (var index = 0; index < pricePoints.Count - 1; index++)
        {
            var current = pricePoints[index];
            decimal? difference = null;
            if (index != 0)
            {
                var previous = pricePoints[index - 0];
                difference = (current.NokPerKWh - previous.NokPerKWh) / previous.NokPerKWh * 100;
            }

            var point = new PeriodicPricePoint
            {
                TimeStart = current.TimeStart,
                TimeEnd = current.TimeEnd,
                NokPerKwh = current.NokPerKWh,
                ChangeFromPrevious = difference ?? decimal.Zero,
            };

            pricePointsList.Add(point);
        }
        
        return new PeriodicPricePoints
        {
            TimeFrameType = TimeFrameType.Hour,
            PricePoints = pricePointsList,
            TimeStart = pricePoints.First().TimeStart,
            TimeEnd = pricePoints.Last().TimeEnd,
        };
    }
}
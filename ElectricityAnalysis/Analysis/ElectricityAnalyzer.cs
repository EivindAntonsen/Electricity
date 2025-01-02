using System.Globalization;
using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Analysis;

public class ElectricityAnalyzer : IElectricityAnalyzer
{
    public IEnumerable<ConsumptionData> CalculateConsumptionData(
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

    public IEnumerable<PeriodicPricePoints> GetPeriodicPricePointsPerTimeFrame(
        TimeFrameType timeFrameType,
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<PricePoint> hourlyPriceDatas)
    {
        var pricePoints = hourlyPriceDatas.OrderBy(pricePoint => pricePoint.TimeStart).ToList();

        return timeFrameType switch
        {
            TimeFrameType.Hour =>
                [ToPeriodicPricePoints(pricePoints)],

            TimeFrameType.Day =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Date)
                    .Select(points => ToPeriodicPricePoints(points.ToList())),

            TimeFrameType.Weekend =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    .Select(points => ToPeriodicPricePoints(points.ToList())),

            TimeFrameType.Week =>
                pricePoints
                    .GroupBy(pricePoint => CultureInfo
                                 .InvariantCulture.DateTimeFormat.Calendar
                                 .GetWeekOfYear(pricePoint.TimeStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    .Select(points => ToPeriodicPricePoints(points.ToList())),

            TimeFrameType.Month =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Month)
                    .Select(points => ToPeriodicPricePoints(points.ToList())),

            TimeFrameType.Season =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Month.ToSeason())
                    .Select(points => ToPeriodicPricePoints(points.ToList())),

            TimeFrameType.Year =>
                pricePoints
                    .GroupBy(pricePoint =>
                                 pricePoint.TimeStart.Year)
                    .Select(points => ToPeriodicPricePoints(points.ToList())),

            _ =>
                pricePoints
                    .GroupBy(pricePoint => CultureInfo
                                 .InvariantCulture.DateTimeFormat.Calendar
                                 .GetWeekOfYear(pricePoint.TimeStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    .Select(points => ToPeriodicPricePoints(points.ToList()))
        };
    }


    private static PeriodicPricePoints ToPeriodicPricePoints(List<PricePoint> pricePoints, TimeFrameType timeFrameType = TimeFrameType.Hour)
    {
        if (pricePoints.Count == 0)
            throw new ArgumentException("No price points found");
        
        pricePoints.Sort((@this, @that) => @this.TimeStart.CompareTo(@that.TimeStart));
        // Initial "maximum" value for tracking minimum
        var minimum = decimal.MaxValue;

        var periodicPricePoints = pricePoints.Select((current, index) =>
        {
            decimal? changeFromPrevious = null;

            // Calculate difference from the previous point
            if (index > 0)
            {
                var previous = pricePoints[index - 1];
                changeFromPrevious = (current.NokPerKWh - previous.NokPerKWh) / previous.NokPerKWh * 100;
            }

            // Update and store the minimum value seen so far
            if (current.NokPerKWh < minimum)
                minimum = current.NokPerKWh;

            // Calculate percentage change from the minimum
            decimal? changeFromMinimum =
                minimum > decimal.Zero
                    ? ((current.NokPerKWh - minimum) / minimum) * 100
                    : null;

            return new PeriodicPricePoint
            {
                TimeStart = current.TimeStart,
                TimeEnd = current.TimeEnd,
                NokPerKwh = current.NokPerKWh,
                ChangeFromPrevious = changeFromPrevious ?? decimal.Zero,
                ChangeFromMinimum = changeFromMinimum ?? decimal.Zero,
            };
        }).ToList();

        return new PeriodicPricePoints
        {
            TimeFrameType = timeFrameType,
            PricePoints = periodicPricePoints,
            TimeStart = pricePoints.First().TimeStart,
            TimeEnd = pricePoints.Last().TimeEnd,
        };
    }
}
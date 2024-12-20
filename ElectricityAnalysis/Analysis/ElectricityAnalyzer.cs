﻿using ElectricityAnalysis.Models;

namespace ElectricityAnalysis.Analysis;

public class ElectricityAnalyzer : IElectricityAnalyzer
{
    public IEnumerable<HourlyStats> CalculateHourlyStats(
        IEnumerable<MeteringValue> meteringValues,
        IEnumerable<HourlyPriceData> hourlyPriceDatas)
    {
        var hourlyStats =
            from meteringValue in meteringValues
            join hourlyPrice in hourlyPriceDatas
                on meteringValue.TimeStart equals hourlyPrice.TimeStart
            select new HourlyStats(
                meteringValue.TimeStart,
                meteringValue.TimeStart.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Friday or DayOfWeek.Sunday,
                meteringValue.TimeStart.Month.ToSeason().ToString(),
                meteringValue.Value,
                hourlyPrice.NokPerKWh
            );

        return hourlyStats;
    }
}
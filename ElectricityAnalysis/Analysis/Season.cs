namespace ElectricityAnalysis.Analysis;

public enum Season
{
    Winter = 1,
    Spring = 2,
    Summer = 3,
    Autumn = 4
}

public static class SeasonExtensions
{
    public static Season FromInt(this int value) =>
        value switch
        {
            12 or 1 or 2 => Season.Winter,
            3 or 4 or 5 => Season.Spring,
            6 or 7 or 8 => Season.Summer,
            9 or 10 or 11 => Season.Autumn,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
}
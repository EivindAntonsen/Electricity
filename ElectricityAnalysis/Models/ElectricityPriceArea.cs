namespace ElectricityAnalysis.Models;

public enum ElectricityPriceArea
{
    Oslo = 1,
    Kristiansand = 2,
    Trondheim = 3,
    Bodø = 4,
    Bergen = 5
}

public static class ElectricityPriceAreaExtensions
{
    public static string ToAreaCode(this ElectricityPriceArea area) => 
        "NO" + (int)area;
}
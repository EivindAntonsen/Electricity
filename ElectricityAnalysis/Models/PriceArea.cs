namespace ElectricityAnalysis.Models;

public enum PriceArea
{
    Oslo = 1,
    Kristiansand = 2,
    Trondheim = 3,
    Bodø = 4,
    Bergen = 5
}

public static class ElectricityPriceAreaExtensions
{
    public static string ToAreaCode(this PriceArea area) => 
        "NO" + (int)area;
}
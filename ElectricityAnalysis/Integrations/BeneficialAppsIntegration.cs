using System.Net.Http.Json;
using ElectricityAnalysis.Models;
using Microsoft.Extensions.Logging;

namespace ElectricityAnalysis.Integrations;

public class BeneficialAppsIntegration(
    ILogger<BeneficialAppsIntegration> logger
) : IBeneficialAppsIntegration
{
    private readonly HttpClient _httpClient = new HttpClient();

    private static Uri GetPriceDataEndpoint(int year, int month, int day, ElectricityPriceArea area) =>
        new Uri($"https://www.hvakosterstrommen.no/api/v1/prices/{year}/{month:00}-{day:00}_{area.ToAreaCode()}.json", UriKind.Absolute);

    public async Task<IEnumerable<HourlyPriceData>> GetElectricityPriceAsync(
        int year,
        int month,
        int day,
        ElectricityPriceArea area,
        CancellationToken cancellationToken = default)
    {
        var uri = GetPriceDataEndpoint(year, month, day, area);
        logger.LogInformation("Calling price endpoint: {Uri}", uri);
        var httpResponseMessage = await _httpClient.GetAsync(uri, cancellationToken);

        try
        {
            httpResponseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception exception)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(exception, "Error getting price data for {Year}, {Month}, {Day}. Content: {Content}", year, month, day, content);
            throw;
        }

        return await httpResponseMessage
                   .Content
                   .ReadFromJsonAsync<HourlyPriceData[]>(cancellationToken: cancellationToken)
               ?? [];
    }

    public async Task<List<HourlyPriceData>> GetAllHourlyElectricityPricesAsync(
        ElectricityPriceArea area,
        CancellationToken cancellationToken = default)
    {
        var hourlyPriceDatas = new List<HourlyPriceData>();
        for (var year = 2021; year <= DateTime.Now.Year; year++)
        {
            var startMonth = year == 2021 ? 12 : 1;

            for (var month = startMonth; month <= 12; month++)
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);
                var finalDay = Math.Min(DateTime.Now.Day, daysInMonth);

                for (var day = 1; day <= finalDay; day++)
                {
                    var uri = GetPriceDataEndpoint(year, month, day, area);

                    logger.LogDebug("Getting price data for {Year}, {Month}, {Day}", year, month, day);
                    var httpResponseMessage = await _httpClient.GetAsync(uri, cancellationToken);

                    httpResponseMessage.EnsureSuccessStatusCode();

                    var o = await httpResponseMessage
                                .Content
                                .ReadFromJsonAsync<HourlyPriceData[]>(cancellationToken: cancellationToken)
                            ?? [];

                    hourlyPriceDatas.AddRange(o);
                }
            }
        }

        return hourlyPriceDatas;
    }
}
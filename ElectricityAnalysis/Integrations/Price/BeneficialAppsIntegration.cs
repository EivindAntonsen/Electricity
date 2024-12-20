using System.Net.Http.Json;
using ElectricityAnalysis.Models;
using Microsoft.Extensions.Logging;

namespace ElectricityAnalysis.Integrations.Price;

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

        string content;
        try
        {
            httpResponseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception exception)
        {
            content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(exception, "Error getting price data for {Year}, {Month}, {Day}. Content: {Content}", year, month, day, content);
            throw;
        }

        content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        logger.LogDebug("Content: {Content}", content);

        return await httpResponseMessage
                   .Content
                   .ReadFromJsonAsync<HourlyPriceData[]>(cancellationToken: cancellationToken)
               ?? [];
    }
}
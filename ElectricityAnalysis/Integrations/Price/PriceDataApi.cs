using System.Net.Http.Json;
using ElectricityAnalysis.Models;
using Microsoft.Extensions.Logging;

namespace ElectricityAnalysis.Integrations.Price;

public class PriceDataApi(
    ILogger<PriceDataApi> logger
) : IPriceDataApi
{
    private readonly HttpClient _httpClient = new();

    private static Uri GetPriceDataEndpoint(DateOnly date, PriceArea area) =>
        new($"https://www.hvakosterstrommen.no/api/v1/prices/{date.Year}/{date.Month:00}-{date.Day:00}_{area.ToAreaCode()}.json", UriKind.Absolute);

    public async Task<IEnumerable<PricePoint>> GetElectricityPriceAsync(
        DateOnly date,
        PriceArea area,
        CancellationToken cancellationToken = default)
    {
        var uri = GetPriceDataEndpoint(date, area);
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
            logger.LogError(exception, 
                            "Error getting price data for {Date}. Content: {Content}", 
                            date,
                            content);
            throw;
        }

        content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        logger.LogDebug("Content: {Content}", content);

        return await httpResponseMessage
                   .Content
                   .ReadFromJsonAsync<PricePoint[]>(cancellationToken: cancellationToken)
               ?? [];
    }
}
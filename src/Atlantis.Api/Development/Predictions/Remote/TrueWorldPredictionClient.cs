using System.Net.Http.Json;

namespace Atlantis.Api.Development.Predictions.Remote;

public sealed class TrueWorldPredictionClient
{
    private readonly HttpClient
        _httpClient;

    private readonly IConfiguration
        _configuration;

    public TrueWorldPredictionClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient =
            httpClient ??
            throw new ArgumentNullException(
                nameof(httpClient));

        _configuration =
            configuration ??
            throw new ArgumentNullException(
                nameof(configuration));
    }

    public async Task<
        TrueWorldPredictionResponseDto>
        WaitForCompletionAsync(
            TrueWorldPredictionRequestDto request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        var endpoint =
            _configuration[
                "DevelopmentPredictionProcessor:Endpoint"];

        if (string.IsNullOrWhiteSpace(
                endpoint))
        {
            throw new InvalidOperationException(
                "Development prediction processor endpoint " +
                "is not configured.");
        }

        using var response =
            await _httpClient.PostAsJsonAsync(
                endpoint,
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<
                TrueWorldPredictionResponseDto>(
                    cancellationToken:
                        cancellationToken)
            ?? throw new InvalidOperationException(
                "True-world prediction endpoint returned " +
                "no completion payload.");
    }
}
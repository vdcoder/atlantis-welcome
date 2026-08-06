using Microsoft.Extensions.Options;

namespace Atlantis.Api.Development.Predictions.Requests;

public sealed class DevelopmentPredictionRequestOptions
{
    public bool AllowAutomaticRequests
    {
        get;
        init;
    }

    public long FirstSequenceNumber
    {
        get;
        init;
    } = 1;

    public long LastSequenceNumber
    {
        get;
        init;
    } = 20;
}

public sealed class DevelopmentPredictionRequestAuthorization
    : IDevelopmentPredictionRequestAuthorization
{
    private readonly DevelopmentPredictionRequestOptions
        _options;

    public DevelopmentPredictionRequestAuthorization(
        IOptions<DevelopmentPredictionRequestOptions> options)
    {
        ArgumentNullException.ThrowIfNull(
            options);

        _options =
            options.Value;
    }

    public bool IsAuthorized(
        string developmentWorldId,
        string scenarioId,
        string simulatedCitizenId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            developmentWorldId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            scenarioId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            simulatedCitizenId);

        return _options.AllowAutomaticRequests;
    }
}
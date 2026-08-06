namespace Atlantis.Api.Development.Predictions.Requests;

public interface IDevelopmentPredictionRequestAuthorization
{
    bool IsAuthorized(
        string developmentWorldId,
        string scenarioId,
        string simulatedCitizenId);
}
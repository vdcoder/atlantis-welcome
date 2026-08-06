namespace Atlantis.Api.World.EmbodiedControl;

public interface IEmbodiedController
{
    Task<PredictionBatch> ProducePredictionsAsync(
        EmbodiedControllerContext context,
        CancellationToken cancellationToken = default);
}
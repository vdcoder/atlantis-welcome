namespace Atlantis.Api.Development.Predictions;

public sealed class DevelopmentPredictionReplayCursor
{
    private readonly object
        _sync =
            new();

    private long
        _nextSequenceNumber =
            1;

    public long GetNextSequenceNumber()
    {
        lock (_sync)
        {
            return _nextSequenceNumber;
        }
    }

    public void Advance(
        long replayedSequenceNumber)
    {
        lock (_sync)
        {
            if (_nextSequenceNumber !=
                replayedSequenceNumber)
            {
                throw new InvalidOperationException(
                    $"Cannot advance replay cursor from " +
                    $"sequence {_nextSequenceNumber} after " +
                    $"replaying sequence " +
                    $"{replayedSequenceNumber}.");
            }

            _nextSequenceNumber++;
        }
    }
}
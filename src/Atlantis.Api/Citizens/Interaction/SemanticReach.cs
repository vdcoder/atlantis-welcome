using Atlantis.Api.Citizens.Perception;

namespace Atlantis.Api.Citizens.Interaction;

public sealed record TargetMatch(
    string EntityId,
    string Reference,
    float Distance);

public sealed record TargetResolution(
    TargetResolutionStatus Status,
    TargetMatch? Match);

public enum TargetResolutionStatus
{
    NotFound = 0,
    Found = 1,
    MultipleFound = 2
}

public sealed class SemanticReach
{
    public TargetResolution Resolve(
        string query,
        IReadOnlyList<PerceivedEntityBinding> nearbyEntityBindings)
    {
        ArgumentNullException.ThrowIfNull(
            nearbyEntityBindings);

        if (string.IsNullOrWhiteSpace(query))
        {
            return new TargetResolution(
                TargetResolutionStatus.NotFound,
                null);
        }

        var queryKeywords =
            Tokenize(
                Normalize(query));

        var matches =
            nearbyEntityBindings
                .Where(
                    binding =>
                        binding.TransparentEntity
                            .IsInteractable)
                .Where(
                    binding =>
                        Matches(
                            queryKeywords,
                            binding.TransparentEntity
                                .Reference))
                .ToList();

        if (matches.Count == 0)
        {
            return new TargetResolution(
                TargetResolutionStatus.NotFound,
                null);
        }

        if (matches.Count > 1)
        {
            return new TargetResolution(
                TargetResolutionStatus.MultipleFound,
                null);
        }

        var binding =
            matches[0];

        return new TargetResolution(
            TargetResolutionStatus.Found,
            new TargetMatch(
                EntityId:
                    binding.EntityId,

                Reference:
                    binding.TransparentEntity
                        .Reference,

                Distance:
                    binding.TransparentEntity
                        .Distance));
    }

    private static bool Matches(
        IReadOnlySet<string> queryKeywords,
        string reference)
    {
        var candidateKeywords =
            Tokenize(
                Normalize(reference));

        return queryKeywords.All(
            candidateKeywords.Contains);
    }

    private static HashSet<string> Tokenize(
        string value)
    {
        return value
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(
                StringComparer.Ordinal);
    }

    private static string Normalize(
        string value)
    {
        return string.Join(
            ' ',
            value
                .Trim()
                .ToLowerInvariant()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }
}
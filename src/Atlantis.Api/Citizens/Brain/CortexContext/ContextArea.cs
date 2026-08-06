namespace Atlantis.Api.Citizens.Brain.CortexContext;

/// <summary>
/// A context area is created for one cognitive breath,
/// generated once, and treated as immutable afterward.
/// </summary>
public abstract class ContextArea<TGenerator>
    where TGenerator :
        class,
        IContextGenerator
{
    private readonly IReadOnlyList<TGenerator>
        _generators;

    private string? _generated;
    protected ContextArea(
        IEnumerable<TGenerator> generators)
    {
        ArgumentNullException.ThrowIfNull(
            generators);

        var materialized =
            generators
                .Select(
                    (generator, registrationOrder) =>
                        new
                        {
                            Generator =
                                generator ??
                                throw new ArgumentException(
                                    "Context generators cannot contain null.",
                                    nameof(generators)),

                            RegistrationOrder =
                                registrationOrder
                        })
                .ToArray();

        var duplicateType =
            materialized
                .GroupBy(item =>
                    item.Generator.GetType())
                .FirstOrDefault(group =>
                    group.Count() > 1);

        if (duplicateType is not null)
        {
            throw new ArgumentException(
                $"Context area contains more than one generator " +
                $"of type '{duplicateType.Key.Name}'.",
                nameof(generators));
        }

        _generators =
            materialized
                .OrderBy(item =>
                    item.Generator.PrefixStabilityHint)
                .ThenBy(item =>
                    item.RegistrationOrder)
                .Select(item =>
                    item.Generator)
                .ToArray();
    }

    public IReadOnlyList<TGenerator>
        Generators =>
            _generators;

    public TRequested? FindGenerator<TRequested>()
        where TRequested :
            class,
            TGenerator
    {
        var matches =
            _generators
                .OfType<TRequested>()
                .ToArray();

        return matches.Length switch
        {
            0 =>
                null,

            1 =>
                matches[0],

            _ =>
                throw new InvalidOperationException(
                    $"Context area contains more than one generator " +
                    $"assignable to '{typeof(TRequested).Name}'.")
        };
    }

    public TRequested GetRequiredGenerator<TRequested>()
        where TRequested :
            class,
            TGenerator
    {
        return FindGenerator<TRequested>()
            ?? throw new InvalidOperationException(
                $"Context generator '{typeof(TRequested).Name}' " +
                "was not found.");
    }

    public async ValueTask<string> GenerateAsync(
        CancellationToken cancellationToken = default)
    {
        if (_generated is not null)
        {
            return _generated;
        }

        var writer =
            new ContextWriter();

        foreach (var generator in _generators)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            await generator.GenerateAsync(
                writer,
                cancellationToken);
        }

        _generated =
            writer.ToString();

        return _generated;
    }
}
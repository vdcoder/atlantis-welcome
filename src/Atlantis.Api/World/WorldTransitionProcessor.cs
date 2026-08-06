using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.World;

public sealed class WorldTransitionProcessor
{
    private readonly WorldPersistenceService
        _persistenceService;

    private readonly SemaphoreSlim
        _gate =
            new(1, 1);

    public WorldTransitionProcessor(
        WorldPersistenceService persistenceService)
    {
        _persistenceService =
            persistenceService ??
            throw new ArgumentNullException(
                nameof(persistenceService));
    }

    public async Task ApplyAsync(
        IReadOnlyList<WorldTransition> transitions,
        WorldState state,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            transitions);

        ArgumentNullException.ThrowIfNull(
            state);

        if (transitions.Count == 0)
        {
            return;
        }

        await _gate.WaitAsync(
            cancellationToken);

        try
        {
            foreach (var transition in transitions)
            {
                ArgumentNullException.ThrowIfNull(
                    transition);

                ApplyTransition(
                    transition,
                    state);
            }

            state.AdvanceRevision();

            await _persistenceService.SaveAsync(
                state,
                cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ApplyAsync(
        WorldTransition transition,
        WorldState state,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            transition);

        ArgumentNullException.ThrowIfNull(
            state);

        await _gate.WaitAsync(
            cancellationToken);

        try
        {
            ApplyTransition(
                transition,
                state);

            state.AdvanceRevision();

            await _persistenceService.SaveAsync(
                state,
                cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static void ApplyTransition(
        WorldTransition transition,
        WorldState state)
    {
        switch (transition)
        {
            case EntityMovedTransition movement:
                Apply(
                    movement,
                    state);
                break;

            case EntitySpokeTransition speech:
                Apply(
                    speech,
                    state);
                break;

            case PrivateMessageDeliveredTransition message:
                Apply(
                    message,
                    state);
                break;

            case UiInputReceivedTransition uiInput:
                Apply(
                    uiInput,
                    state);
                break;

            case EntityTorsoTurnedTransition torsoTurned:
                Apply(
                    torsoTurned,
                    state);
                break;

            case EntityGazeChangedTransition gazeChanged:
                Apply(
                    gazeChanged,
                    state);
                break;

            default:
                throw new NotSupportedException(
                    $"Unsupported transition: " +
                    $"{transition.GetType().Name}");
        }
    }

    private static void Apply(
        EntityMovedTransition transition,
        WorldState state)
    {
        var entity =
            state.World.Entities
                .Single(
                    entity =>
                        entity.Id ==
                        transition.EntityId);

        entity.Position =
            transition.To;
    }

    private static void Apply(
        EntitySpokeTransition transition,
        WorldState state)
    {
        var entity =
            state.World.Entities
                .Single(
                    entity =>
                        entity.Id ==
                        transition.EntityId);

        entity.CurrentUtterance =
            transition.Utterance;
    }

    private static void Apply(
        PrivateMessageDeliveredTransition transition,
        WorldState state)
    {
        var recipient =
            state.World.Entities
                .Single(
                    entity =>
                        entity.Id ==
                        transition.RecipientId);

        recipient.CurrentPrivateMessage =
            transition.Message;
    }

    private static void Apply(
        UiInputReceivedTransition transition,
        WorldState state)
    {
        // No authoritative world-state change yet.
    }

    private static void Apply(
        EntityTorsoTurnedTransition transition,
        WorldState state)
    {
        var entity =
            state.World.Entities
                .Single(
                    entity =>
                        entity.Id ==
                        transition.EntityId);

        if (entity.Embodiment is null)
        {
            throw new InvalidOperationException(
                $"Entity '{entity.Id}' is not embodied.");
        }

        entity.Embodiment.TorsoFront =
            transition.TorsoFront;
    }

    private static void Apply(
        EntityGazeChangedTransition transition,
        WorldState state)
    {
        var entity =
            state.World.Entities
                .Single(
                    entity =>
                        entity.Id ==
                        transition.EntityId);

        if (entity.Embodiment is null)
        {
            throw new InvalidOperationException(
                $"Entity '{entity.Id}' is not embodied.");
        }

        entity.Embodiment.GazeDirection =
            transition.GazeDirection;
    }
}
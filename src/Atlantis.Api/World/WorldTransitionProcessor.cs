using Atlantis.Api.Persistence.Services;
using Atlantis.Api.World.Orbs;
using Atlantis.Api.World.Transitions;

namespace Atlantis.Api.World;

public sealed class WorldTransitionProcessor
{
    private static readonly TimeSpan
        PositionObservationRetention =
            TimeSpan.FromSeconds(3);

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

            case SensoryOrbCreatedTransition orbCreated:
                Apply(
                    orbCreated,
                    state);
                break;

            case EntityPositionReportedTransition positionReported:
                Apply(
                    positionReported,
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

        entity.Position = transition.To;
        entity.PositionChangedAt = transition.ChangedAt;
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

    private static void Apply(
        SensoryOrbCreatedTransition transition,
        WorldState state)
    {
        state.World.Orbs.Add(
            transition.Orb);
    }

    private static void Apply(
        EntityPositionReportedTransition reported,
        WorldState state)
    {
        var entity =
        state.World.Entities
            .SingleOrDefault(
                value =>
                    value.Id == reported.EntityId)
        ?? throw new EntityNotFoundException(
            reported.EntityId);

        if (entity.Position.Equals(reported.ReportedPosition))
        {
            return;
        }

        var positionFirstRecordedAt =
            entity.PositionChangedAt <= reported.ObservedAt
                ? entity.PositionChangedAt
                : reported.ObservedAt;

        var historicalOrb =
            new PositionObservationOrb(
                id:
                    Guid.NewGuid(),

                createdAt:
                    reported.ObservedAt,

                expiresAt:
                    reported.ObservedAt +
                    PositionObservationRetention,

                sourceEntityId:
                    reported.SourceEntityId,

                observedEntityId:
                    entity.Id,

                observedPosition:
                    entity.Position,

                positionFirstRecordedAt:
                    positionFirstRecordedAt);

        state.World.Orbs.Add(
            historicalOrb);

        entity.Position =
            reported.ReportedPosition;

        entity.PositionChangedAt =
            reported.ObservedAt;
    }
}
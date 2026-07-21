using System;
using System.Collections;
using System.Collections.Generic;
using AtlantisWelcome.Entities;
using AtlantisWelcome.Networking;
using UnityEngine;

namespace AtlantisWelcome.World
{
    public sealed class WorldSnapshotLoader : MonoBehaviour
    {
        [SerializeField]
        private AtlantisApiClient apiClient;

        [SerializeField]
        private EntityView citizenPrefab;

        [SerializeField]
        private Transform entityContainer;

        [SerializeField]
        private float refreshIntervalSeconds = 1f;

        private readonly Dictionary<string, EntityView>
            _entityViews = new();

        private readonly Dictionary<string, long>
            _latestUtteranceSequences = new();

        private readonly Dictionary<string, long>
            _latestPrivateMessageSequences = new();

        private bool _hasAppliedInitialSnapshot;

        public long CurrentRevision { get; private set; } = -1;

        public event Action<EntityDto, UtteranceDto>
            UtteranceReceived;

        public event Action<EntityDto, PrivateMessageDto>
            PrivateMessageReceived;

        private void Start()
        {
            StartCoroutine(RefreshLoop());
        }

        private IEnumerator RefreshLoop()
        {
            while (true)
            {
                yield return apiClient.GetWorldSnapshot(
                    ApplySnapshot,
                    HandleFailure);

                yield return new WaitForSeconds(
                    refreshIntervalSeconds);
            }
        }

        public bool TryGetEntityView(
            string entityId,
            out EntityView entityView)
        {
            return _entityViews.TryGetValue(
                entityId,
                out entityView);
        }

        public EntityView FindEntityView(string entityId)
        {
            return _entityViews.TryGetValue(
                entityId,
                out var entityView)
                    ? entityView
                    : null;
        }

        private void ApplySnapshot(
            WorldSnapshotDto snapshot)
        {
            if (snapshot.revision <= CurrentRevision)
            {
                return;
            }

            CurrentRevision = snapshot.revision;

            foreach (var entity in snapshot.world.entities)
            {
                ApplyEntity(entity);
                ApplyCommunications(entity);
            }

            _hasAppliedInitialSnapshot = true;

            Debug.Log(
                $"Atlantis world '{snapshot.world.worldId}' " +
                $"revision {CurrentRevision} loaded with " +
                $"{snapshot.world.entities.Length} entities.");
        }

        private void ApplyEntity(EntityDto entity)
        {
            if (!_entityViews.TryGetValue(
                    entity.id,
                    out var entityView))
            {
                entityView = Instantiate(
                    citizenPrefab,
                    entityContainer);

                _entityViews.Add(
                    entity.id,
                    entityView);
            }

            entityView.Apply(entity);
        }

        private void ApplyCommunications(
            EntityDto entity)
        {
            ApplyUtterance(entity);
            ApplyPrivateMessage(entity);
        }

        private void ApplyUtterance(
            EntityDto entity)
        {
            var utterance = entity.currentUtterance;

            if (utterance == null)
            {
                return;
            }

            var previousSequence =
                _latestUtteranceSequences.TryGetValue(
                    entity.id,
                    out var sequence)
                        ? sequence
                        : -1;

            _latestUtteranceSequences[entity.id] =
                utterance.sequence;

            if (!_hasAppliedInitialSnapshot)
            {
                return;
            }

            if (utterance.sequence <= previousSequence)
            {
                return;
            }

            //Debug.Log(
            //    $"{entity.name} said: {utterance.text}");

            UtteranceReceived?.Invoke(
                entity,
                utterance);
        }

        private void ApplyPrivateMessage(
            EntityDto entity)
        {
            var message =
                entity.currentPrivateMessage;

            if (message == null)
            {
                return;
            }

            var previousSequence =
                _latestPrivateMessageSequences.TryGetValue(
                    entity.id,
                    out var sequence)
                        ? sequence
                        : -1;

            _latestPrivateMessageSequences[entity.id] =
                message.sequence;

            if (!_hasAppliedInitialSnapshot)
            {
                return;
            }

            if (message.sequence <= previousSequence)
            {
                return;
            }

            //Debug.Log(
            //    $"{entity.name} received a private touch " +
            //    $"from {message.senderId}.");

            PrivateMessageReceived?.Invoke(
                entity,
                message);
        }

        private static void HandleFailure(string message)
        {
            Debug.LogError(message);
        }
    }
}
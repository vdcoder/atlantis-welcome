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

        private readonly Dictionary<string, EntityView> _entityViews = new();

        public long CurrentRevision { get; private set; } = -1;

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

                yield return new WaitForSeconds(refreshIntervalSeconds);
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

        public EntityView? FindEntityView(string entityId)
        {
            return _entityViews.TryGetValue(
                entityId,
                out var entityView)
                    ? entityView
                    : null;
        }

        private void ApplySnapshot(WorldSnapshotDto snapshot)
        {
            if (snapshot.revision <= CurrentRevision)
            {
                return;
            }

            CurrentRevision = snapshot.revision;

            foreach (var entity in snapshot.world.entities)
            {
                ApplyEntity(entity);
            }

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

                _entityViews.Add(entity.id, entityView);
            }

            entityView.Apply(entity);
        }

        private static void HandleFailure(string message)
        {
            Debug.LogError(message);
        }
    }
}
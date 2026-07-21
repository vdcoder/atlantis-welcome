using AtlantisWelcome.World;
using UnityEngine;

namespace AtlantisWelcome.Entities
{
    public sealed class EntityView : MonoBehaviour
    {
        [SerializeField]
        private float interpolationDurationSeconds = 0.8f;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _interpolationStartedAt;
        private bool _isInterpolating;

        public string EntityId { get; private set; } = string.Empty;

        public Vector3 AuthoritativePosition { get; private set; }

        public void Apply(EntityDto entity)
        {
            var receivedPosition = new Vector3(
                entity.position.x,
                entity.position.y,
                entity.position.z);

            var isNewEntity = string.IsNullOrEmpty(EntityId);

            EntityId = entity.id;
            gameObject.name = entity.name;

            AuthoritativePosition = receivedPosition;

            if (isNewEntity)
            {
                transform.position = receivedPosition;
                _startPosition = receivedPosition;
                _targetPosition = receivedPosition;
                _isInterpolating = false;
                return;
            }

            _startPosition = transform.position;
            _targetPosition = receivedPosition;
            _interpolationStartedAt = Time.time;
            _isInterpolating = true;
        }

        private void Update()
        {
            if (!_isInterpolating)
            {
                return;
            }

            var elapsed = Time.time - _interpolationStartedAt;

            var progress = interpolationDurationSeconds <= 0f
                ? 1f
                : Mathf.Clamp01(
                    elapsed / interpolationDurationSeconds);

            transform.position = Vector3.Lerp(
                _startPosition,
                _targetPosition,
                progress);

            if (progress >= 1f)
            {
                transform.position = _targetPosition;
                _isInterpolating = false;
            }
        }
    }
}
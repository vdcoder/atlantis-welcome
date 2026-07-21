using System.Collections;
using AtlantisWelcome.Entities;
using AtlantisWelcome.Networking;
using AtlantisWelcome.World;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AtlantisWelcome.Interaction
{
    public sealed class FirstPersonWorldController : MonoBehaviour
    {
        [Header("Dependencies")]

        [SerializeField]
        private AtlantisApiClient apiClient;

        [SerializeField]
        private WorldSnapshotLoader worldLoader;

        [SerializeField]
        private Camera playerCamera;

        [Header("Controlled Entity")]

        [SerializeField]
        private string actorId = "orestes";

        [SerializeField]
        private string entityId = "orestes";

        [Header("Movement")]

        [SerializeField]
        private float movementStep = 1f;

        [SerializeField]
        private float groundHeight = 0f;

        [Header("View")]

        [SerializeField]
        private float eyeHeight = 1.65f;

        [SerializeField]
        private float mouseSensitivity = 2f;

        [SerializeField]
        private float maximumPitch = 85f;

        private float _yaw;
        private float _pitch;
        private bool _movementRequestInFlight;

        private void Start()
        {
            _yaw = transform.eulerAngles.y;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            UpdateMouseLook();
            UpdateCameraPosition();
            ReadMovementInput();
            ReadCursorInput();
        }

        private void UpdateMouseLook()
        {
            var mouseX =
                Input.GetAxis("Mouse X") * mouseSensitivity;

            var mouseY =
                Input.GetAxis("Mouse Y") * mouseSensitivity;

            _yaw += mouseX;
            _pitch -= mouseY;

            _pitch = Mathf.Clamp(
                _pitch,
                -maximumPitch,
                maximumPitch);

            playerCamera.transform.rotation =
                Quaternion.Euler(_pitch, _yaw, 0f);
        }

        private void UpdateCameraPosition()
        {
            var entityView =
                worldLoader.FindEntityView(entityId);

            if (entityView == null)
            {
                return;
            }

            playerCamera.transform.position =
                entityView.transform.position +
                Vector3.up * eyeHeight;
        }

        public void MoveForward()
        {
            RequestMovement(Vector3.forward);
        }

        public void MoveBackward()
        {
            RequestMovement(Vector3.back);
        }

        public void MoveLeft()
        {
            RequestMovement(Vector3.left);
        }

        public void MoveRight()
        {
            RequestMovement(Vector3.right);
        }

        private void ReadMovementInput()
        {
            if (_movementRequestInFlight)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveForward();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MoveBackward();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MoveRight();
            }
        }

        private void RequestMovement(Vector3 localDirection)
        {
            var entityView =
                worldLoader.FindEntityView(entityId);

            if (entityView == null)
            {
                Debug.LogWarning(
                    $"Cannot move '{entityId}' because its view " +
                    "has not been loaded.");

                return;
            }

            var yawRotation =
                Quaternion.Euler(0f, _yaw, 0f);

            var worldDirection =
                yawRotation * localDirection;

            worldDirection.y = 0f;
            worldDirection.Normalize();

            var destination =
                entityView.AuthoritativePosition +
                worldDirection * movementStep;

            destination.y = groundHeight;

            //Debug.Log(
            //    $"Calculated movement: " +
            //    $"authoritative={entityView.AuthoritativePosition}, " +
            //    $"direction={worldDirection}, " +
            //    $"destination={destination}");

            StartCoroutine(
                SubmitMovement(destination));
        }

        private IEnumerator SubmitMovement(
            Vector3 destination)
        {
            _movementRequestInFlight = true;

            yield return apiClient.MoveEntity(
                actorId,
                entityId,
                destination,
                result =>
                {
                    Debug.Log(
                        $"Movement approved at revision " +
                        $"{result.revision}: " +
                        $"{result.entityId} → " +
                        $"({result.position.x}, " +
                        $"{result.position.y}, " +
                        $"{result.position.z})");
                },
                error =>
                {
                    Debug.LogError(error);
                });

            _movementRequestInFlight = false;
        }

        private static void ReadCursorInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Input.GetMouseButtonDown(0) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
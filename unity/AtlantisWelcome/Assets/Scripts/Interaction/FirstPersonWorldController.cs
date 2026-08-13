using AtlantisWelcome.World;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AtlantisWelcome.Interaction
{
    public sealed class FirstPersonWorldController
        : MonoBehaviour
    {
        [Header("Dependencies")]

        [SerializeField]
        private WorldSnapshotLoader worldLoader;

        [SerializeField]
        private Camera playerCamera;

        [Header("Controlled Entity")]

        [SerializeField]
        private string entityId =
            "visitor-default";

        [Header("Movement")]

        [SerializeField]
        private float movementSpeedMetersPerSecond =
            3f;

        [SerializeField]
        private float groundHeight =
            0f;

        [Header("View")]

        [SerializeField]
        private float eyeHeight =
            1.65f;

        [SerializeField]
        private float mouseSensitivity =
            2f;

        [SerializeField]
        private float maximumPitch =
            85f;

        private float _yaw;
        private float _pitch;

        private void Start()
        {
            _yaw =
                transform.eulerAngles.y;

            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible =
                false;
        }

        private void Update()
        {
            UpdateMouseLook();
            ReadMovementInput();
            UpdateCameraPosition();
            ReadCursorInput();
        }

        private void ReadMovementInput()
        {
            var input =
                new Vector3(
                    Input.GetAxisRaw("Horizontal"),
                    0f,
                    Input.GetAxisRaw("Vertical"));

            if (input.sqrMagnitude <= 0f)
            {
                return;
            }

            input.Normalize();

            var yawRotation =
                Quaternion.Euler(
                    0f,
                    _yaw,
                    0f);

            var worldDirection =
                yawRotation * input;

            var entityView =
                worldLoader.FindEntityView(
                    entityId);

            if (entityView == null)
            {
                return;
            }

            var movement =
                worldDirection *
                movementSpeedMetersPerSecond *
                Time.deltaTime;

            var nextPosition =
                entityView.transform.position +
                movement;

            nextPosition.y =
                groundHeight;

            entityView.transform.position =
                nextPosition;
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

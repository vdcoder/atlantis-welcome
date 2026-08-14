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
        private float mouseWheelStepMeters =
            0.35f;

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
            ReadArrowMovementInput();
            ReadMouseWheelMovement();
            UpdateCameraPosition();
            ReadCursorInput();
        }

        private void ReadArrowMovementInput()
        {
            var horizontal =
                0f;

            var vertical =
                0f;

            if (Input.GetKey(
                    KeyCode.LeftArrow))
            {
                horizontal -= 1f;
            }

            if (Input.GetKey(
                    KeyCode.RightArrow))
            {
                horizontal += 1f;
            }

            if (Input.GetKey(
                    KeyCode.UpArrow))
            {
                vertical += 1f;
            }

            if (Input.GetKey(
                    KeyCode.DownArrow))
            {
                vertical -= 1f;
            }

            var input =
                new Vector3(
                    horizontal,
                    0f,
                    vertical);

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
                yawRotation *
                input;

            MoveControlledEntity(
                worldDirection *
                movementSpeedMetersPerSecond *
                Time.deltaTime);
        }

        private void ReadMouseWheelMovement()
        {
            var scroll =
                Input.mouseScrollDelta.y;

            if (Mathf.Approximately(
                    scroll,
                    0f))
            {
                return;
            }

            var direction =
                scroll > 0f
                    ? 1f
                    : -1f;

            var yawRotation =
                Quaternion.Euler(
                    0f,
                    _yaw,
                    0f);

            var forward =
                yawRotation *
                Vector3.forward;

            MoveControlledEntity(
                forward *
                mouseWheelStepMeters *
                direction);
        }

        private void MoveControlledEntity(
            Vector3 movement)
        {
            var entityView =
                worldLoader.FindEntityView(
                    entityId);

            if (entityView == null)
            {
                return;
            }

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
                Input.GetAxis("Mouse X") *
                mouseSensitivity;

            var mouseY =
                Input.GetAxis("Mouse Y") *
                mouseSensitivity;

            _yaw +=
                mouseX;

            _pitch -=
                mouseY;

            _pitch =
                Mathf.Clamp(
                    _pitch,
                    -maximumPitch,
                    maximumPitch);

            playerCamera.transform.rotation =
                Quaternion.Euler(
                    _pitch,
                    _yaw,
                    0f);
        }

        private void UpdateCameraPosition()
        {
            var entityView =
                worldLoader.FindEntityView(
                    entityId);

            if (entityView == null)
            {
                return;
            }

            playerCamera.transform.position =
                entityView.transform.position +
                Vector3.up *
                eyeHeight;
        }

        private static void ReadCursorInput()
        {
            if (Input.GetKeyDown(
                    KeyCode.Escape))
            {
                Cursor.lockState =
                    CursorLockMode.None;

                Cursor.visible =
                    true;
            }

            if (Input.GetMouseButtonDown(0) &&
                !EventSystem.current
                    .IsPointerOverGameObject())
            {
                Cursor.lockState =
                    CursorLockMode.Locked;

                Cursor.visible =
                    false;
            }
        }
    }
}
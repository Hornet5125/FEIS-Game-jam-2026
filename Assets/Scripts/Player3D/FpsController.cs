
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FpsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float jumpHeight = 1.1f;
    [SerializeField] private float gravity = -20f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 2f;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                playerCamera = cam.transform;
        }
    }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        HandleMouse();
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
            LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleMouse()
    {
        if (Cursor.lockState != CursorLockMode.Locked ||
            playerCamera == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0f, mouseX, 0f);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);

        playerCamera.localRotation =
            Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction =
            transform.right * x + transform.forward * z;

        direction = Vector3.ClampMagnitude(direction, 1f);

        bool sprinting =
            Input.GetKey(KeyCode.LeftShift) && z > 0f;

        float speed = sprinting ? sprintSpeed : walkSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (controller.isGrounded &&
            Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity =
            direction * speed + Vector3.up * verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}

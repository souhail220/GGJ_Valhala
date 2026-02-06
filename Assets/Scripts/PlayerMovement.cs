using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 6f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;
    public Transform cameraHolder;

    [Header("Global Gravity")]
    [Range(0f, 100f)]
    public float gravityLevel = 50f; // 0 = fly, 100 = no jump

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;

    // Input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ReadInput();
        Move();
        Look();
    }

    void ReadInput()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        moveInput = Vector2.zero;

        if (keyboard.wKey.isPressed) moveInput.y += 1;
        if (keyboard.sKey.isPressed) moveInput.y -= 1;
        if (keyboard.aKey.isPressed) moveInput.x -= 1;
        if (keyboard.dKey.isPressed) moveInput.x += 1;

        lookInput = mouse.delta.ReadValue();

        jumpPressed = keyboard.spaceKey.wasPressedThisFrame;
    }

    void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        // ?? SPEED SCALES WITH GRAVITY
        float gravityFactor = 1f - (gravityLevel / 100f);
        float currentSpeed = Mathf.Lerp(1f, moveSpeed, gravityFactor);

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump (disabled when gravity is very high)
        if (jumpPressed && grounded && gravityLevel < 90f)
        {
            velocity.y = jumpForce * gravityFactor;
        }

        // Gravity force
        float gravity = Mathf.Lerp(0f, -50f, gravityLevel / 100f);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}

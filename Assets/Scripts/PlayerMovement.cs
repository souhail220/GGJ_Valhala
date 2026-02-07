using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 0.5f;
    
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Ground Detection")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.3f;
    
    // Input System
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    
    // Movement state
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;
    private float currentSpeed;
    
    // Camera rotation
    private float xRotation = 0f;
    
    // Components
    private CharacterController characterController;
    
    void Start()
    {
        // Get or add CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }
        
        // Setup camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Set up Input System
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            jumpAction = playerInput.actions["Jump"];
            sprintAction = playerInput.actions["Sprint"];
        }
        else
        {
            Debug.LogWarning("PlayerInput component not found on " + gameObject.name);
        }
    }
    
    void Update()
    {
        // Read inputs
        if (moveAction != null) moveInput = moveAction.ReadValue<Vector2>();
        if (lookAction != null) lookInput = lookAction.ReadValue<Vector2>();
        if (sprintAction != null) isSprinting = sprintAction.IsPressed();
        
        // Ground check
        CheckGround();
        
        // Handle movement
        HandleMovement();
        
        // Handle mouse look
        HandleMouseLook();
        
        // Handle jump
        if (jumpAction != null && jumpAction.WasPerformedThisFrame() && isGrounded)
        {
            Jump();
        }
        
        // Apply drag
        ApplyDrag();
    }
    
    void CheckGround()
    {
        // Raycast down to check if grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundCheckDistance, groundMask);
    }
    
    void HandleMovement()
    {
        // Calculate movement direction
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        // Determine speed
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        
        // Move the character
        characterController.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
        
        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // Keep grounded
        }
        
        characterController.Move(velocity * Time.deltaTime);
    }
    
    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        
        // Rotate camera up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate player left/right
        transform.Rotate(Vector3.up * mouseX);
    }
    
    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        Debug.Log("Player jumped!");
    }
    
    void ApplyDrag()
    {
        // Apply drag based on ground state
        float drag = isGrounded ? groundDrag : airDrag;
        velocity.x *= 1f - drag * Time.deltaTime;
        velocity.z *= 1f - drag * Time.deltaTime;
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize ground check
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + groundCheckDistance));
    }
}

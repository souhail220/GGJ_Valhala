using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Controlplayer : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public float Right;
    public float Left;
    public float moveSpeed = 5f;
    public float MouseSensitivity = 1f;
    public float jumpForce = 5f;
    public float deadZone = 15;
    public bool isGrounded = false;

    void Start()
    {
        if (myRigidbody == null)
        {
            myRigidbody = GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // Mouse look (frame-rate independent)
        if (Input.GetMouseButton(0))
        {
            transform.eulerAngles += MouseSensitivity * Time.deltaTime * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        }

        // Jump input - store input here, apply in FixedUpdate
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            myRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Reset scene if falling too far
        if (transform.position.y > deadZone)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void FixedUpdate()
    {
        // Read axes (works with keyboard or controller)
        float h = Input.GetAxis("Horizontal"); // A/D or Left/Right (or Q/D on AZERTY)
        float v = Input.GetAxis("Vertical");   // W/S or Up/Down (or Z/S on AZERTY)

        // Movement relative to player orientation
        Vector3 desiredMove = (transform.forward * v + transform.right * h) * moveSpeed;

        // Apply movement using Rigidbody (preserves physics)
        Vector3 newPosition = myRigidbody.position + desiredMove * Time.fixedDeltaTime;
        myRigidbody.MovePosition(newPosition);

        // Optional: discrete key-based rotation that mirrors previous behavior
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(transform.up, h * Right * Time.fixedDeltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(transform.up, -h * Left * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only consider ground collisions for grounding
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

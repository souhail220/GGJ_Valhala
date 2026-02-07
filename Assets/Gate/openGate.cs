using UnityEngine;
using UnityEngine.InputSystem;

public class openGate : MonoBehaviour
{
    public Transform door01;
    public Transform door02;

    public float door1OpenX = 0.85f;
    public float door2OpenX = -0.85f;
    public float speed = 3f;

    
    public AudioSource doorAudio; // assign in Inspector
    public float audioPitch = 2f; // 2x speed

    private Vector3 door1ClosedPos;
    private Vector3 door2ClosedPos;

    private bool isOpen = false;
    private bool playerInside = false;
    private bool canInteract = false;

    void Start()
    {
        door1ClosedPos = door01.localPosition;
        door2ClosedPos = door02.localPosition;

        if (doorAudio != null)
            doorAudio.pitch = audioPitch; // speed up sound
    }

    void Update()
    {
        // Open only (no manual close)
        if (canInteract && Keyboard.current.eKey.wasPressedThisFrame && !isOpen)
        {
            isOpen = true;
            PlayDoorSound();
        }

        MoveDoors();
    }

    void MoveDoors()
    {
        Vector3 door1Target = isOpen
            ? new Vector3(door1OpenX, door1ClosedPos.y, door1ClosedPos.z)
            : door1ClosedPos;

        Vector3 door2Target = isOpen
            ? new Vector3(door2OpenX, door2ClosedPos.y, door2ClosedPos.z)
            : door2ClosedPos;

        door01.localPosition = Vector3.Lerp(
            door01.localPosition, door1Target, Time.deltaTime * speed);

        door02.localPosition = Vector3.Lerp(
            door02.localPosition, door2Target, Time.deltaTime * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            canInteract = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            canInteract = false;

            // Auto-close after player leaves
            if (isOpen)
            {
                isOpen = false;
                PlayDoorSound();
            }
        }
    }

    void PlayDoorSound()
    {
        if (doorAudio != null)
        {
            doorAudio.Stop(); // restart if already playing
            doorAudio.Play();
        }
    }
}

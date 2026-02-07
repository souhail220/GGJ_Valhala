using UnityEngine;
using UnityEngine.InputSystem; // NEW INPUT SYSTEM

public class InteractionPrompt : MonoBehaviour
{
    public GameObject pressEText;
    public Animator npcAnimator;
    private bool canInteract = false;
    private bool animationPlaying = false;

    void Start()
    {
        pressEText.SetActive(false);
    }

    void Update()
    {
        // ✅ Only respond to E if inside trigger AND animation not playing
        if (canInteract && !animationPlaying && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    void Interact()
    {
        animationPlaying = true;
        pressEText.SetActive(false);
        npcAnimator.SetTrigger("Interact");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("interactionTrigger"))
        {
            canInteract = true;
            if (!animationPlaying)
                pressEText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("interactionTrigger"))
        {
            canInteract = false;
            pressEText.SetActive(false);
        }
    }

    // Call this from animation event at the end of the animation
    public void AnimationFinished()
    {
        animationPlaying = false;
        if (canInteract)
            pressEText.SetActive(true);
    }
}

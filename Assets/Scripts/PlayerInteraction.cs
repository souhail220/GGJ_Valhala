using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private InteractionTrigger currentTrigger;

    void Update()
    {
        if (currentTrigger != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentTrigger.Interact();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        InteractionTrigger trigger = other.GetComponent<InteractionTrigger>();
        if (trigger != null)
        {
            currentTrigger = trigger;
        }
    }

    void OnTriggerExit(Collider other)
    {
        InteractionTrigger trigger = other.GetComponent<InteractionTrigger>();
        if (trigger != null && trigger == currentTrigger)
        {
            currentTrigger = null;
        }
    }
}

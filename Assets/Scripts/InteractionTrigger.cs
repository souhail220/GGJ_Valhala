using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "Interact";

    public void Interact()
    {
        animator.SetTrigger(triggerName);
    }
}

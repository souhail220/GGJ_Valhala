using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public GameObject pressEText;

    void Start()
    {
        pressEText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("interactionTrigger"))
        {
            pressEText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("interactionTrigger"))
        {
            pressEText.SetActive(false);
        }
    }
}

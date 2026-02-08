using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Image crosshairImage;
    
    void Start()
    {
        if (crosshairImage == null)
        {
            crosshairImage = GetComponent<Image>();
        }
    }
}

using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Material glitchMaterial;
    
    private float glitchIntensity = 0f;
    
    void Start()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
        
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        // Subscribe to glitch changes
        if (playerHealth != null)
        {
            playerHealth.OnGlitchIntensityChanged.AddListener(OnGlitchIntensityChanged);
        }
    }
    
    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnGlitchIntensityChanged.RemoveListener(OnGlitchIntensityChanged);
        }
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (glitchIntensity <= 0 || glitchMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        // Apply glitch shader with intensity
        glitchMaterial.SetFloat("_GlitchIntensity", glitchIntensity);
        Graphics.Blit(source, destination, glitchMaterial);
    }
    
    void OnGlitchIntensityChanged(float intensity)
    {
        glitchIntensity = intensity;
    }
}

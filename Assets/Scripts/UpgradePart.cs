using UnityEngine;

public class UpgradePart : MonoBehaviour
{
    public enum PartType
    {
        HelmetOxygenTank,
        RadiationProtection
    }
    
    [Header("Part Settings")]
    [SerializeField] private PartType partType;
    [SerializeField] private GameObject pickupEffectPrefab;
    [SerializeField] private AudioClip pickupSound;
    
    [Header("Visual")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.3f;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Rotate the part
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player picked up the part
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            bool upgraded = false;
            
            switch (partType)
            {
                case PartType.HelmetOxygenTank:
                    upgraded = playerHealth.UpgradeHelmet();
                    if (upgraded)
                    {
                        Debug.Log("Player collected Helmet/Oxygen Tank upgrade!");
                    }
                    break;
                    
                case PartType.RadiationProtection:
                    upgraded = playerHealth.UpgradeRadiationProtection();
                    if (upgraded)
                    {
                        Debug.Log("Player collected Radiation Protection upgrade!");
                    }
                    break;
            }
            
            if (upgraded)
            {
                // Spawn pickup effect
                if (pickupEffectPrefab != null)
                {
                    GameObject effect = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(effect, 2f);
                }
                
                // Play pickup sound
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }
                
                // Destroy the part
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Already have this upgrade!");
            }
        }
    }
}

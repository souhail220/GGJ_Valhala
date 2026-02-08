using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UpgradePart : MonoBehaviour
{
    public enum PartType
    {
        GreenCore,    // Adds health
        IceCore,      // Adds cold protection
        EnergyCore    // Provides cure ingredient
    }
    
    [Header("Part Settings")]
    [SerializeField] private PartType partType;
    [SerializeField] private float boostAmount = 25f;
    [SerializeField] private GameObject pickupEffectPrefab;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private Collider triggerCollider;
    
    private TextMeshProUGUI pickupPromptText;
    
    [Header("Visual")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.3f;
    
    private Vector3 startPosition;
    private bool canPickup = false;
    private PlayerHealth playerInRange;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Find the trigger collider if not assigned
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider>();
            
            // If not found on this object, search in children
            if (triggerCollider == null)
            {
                triggerCollider = GetComponentInChildren<Collider>();
            }
        }
        
        Debug.Log("Trigger Collider found: " + (triggerCollider != null ? triggerCollider.gameObject.name : "NOT FOUND"));
        
        // Find TextMeshPro text in the trigger collider's children
        if (triggerCollider != null)
        {
            pickupPromptText = triggerCollider.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log("Pickup prompt text found: " + (pickupPromptText != null ? pickupPromptText.gameObject.name : "NOT FOUND"));
        }
        
        // Hide the text initially
        if (pickupPromptText != null)
        {
            pickupPromptText.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        // Rotate the part
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Check for E key press when in range
        if (canPickup && Keyboard.current.eKey.wasPressedThisFrame && playerInRange != null)
        {
            Debug.Log("E pressed! Picking up part...");
            PickupPart(playerInRange);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log("Player detected in range!");
            canPickup = true;
            playerInRange = playerHealth;
            
            // Show the pickup prompt text
            if (pickupPromptText != null)
            {
                pickupPromptText.gameObject.SetActive(true);
                Debug.Log("Pickup prompt shown!");
            }
            else
            {
                Debug.LogWarning("Pickup prompt text not found!");
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log("Player left range!");
            canPickup = false;
            playerInRange = null;
            
            // Hide the pickup prompt text
            if (pickupPromptText != null)
                pickupPromptText.gameObject.SetActive(false);
        }
    }
    
    void PickupPart(PlayerHealth playerHealth)
    {
        switch (partType)
        {
            case PartType.GreenCore:
                playerHealth.AddHealth(boostAmount);
                Debug.Log("Player collected Green Core! Health restored!");
                break;
                
            case PartType.IceCore:
                playerHealth.AddColdResistance(boostAmount);
                Debug.Log("Player collected Ice Core! Cold protection acquired!");
                break;
                
            case PartType.EnergyCore:
                playerHealth.ObtainEnergyCore();
                Debug.Log("Player collected Energy Core! Final cure ingredient obtained!");
                break;
        }
        
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
}

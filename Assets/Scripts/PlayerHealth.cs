using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float currentOxygen;
    
    [Header("Radiation Settings")]
    [SerializeField] private float maxRadiation = 100f;
    [SerializeField] private float currentRadiation = 0f;
    
    [Header("Toxicity Settings")]
    [SerializeField] private float maxToxicity = 100f;
    [SerializeField] private float currentToxicity = 0f;
    
    [Header("Cold Protection Settings")]
    [SerializeField] private float coldResistance = 0f; // 0-100 scale
    [SerializeField] private float maxColdResistance = 100f;
    
    [Header("Cure Ingredient")]
    private bool hasEnergyCore = false;
    
    [Header("Glitch Settings")]
    [SerializeField] private float glitchIntensity = 0f; // 0-1 scale
    [SerializeField] private float maxGlitchIntensity = 1f;
    [SerializeField] private float glitchDecayRate = 0.5f; // How fast glitch fades
    
    [Header("Damage Settings")]
    [SerializeField] private float invincibilityDuration = 1f; // Seconds of i-frames after taking damage
    [SerializeField] private bool isDead = false;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnOxygenChanged;
    public UnityEvent<float> OnRadiationChanged;
    public UnityEvent<float> OnToxicityChanged;
    public UnityEvent<float> OnColdResistanceChanged;
    public UnityEvent OnEnergyCoreObtained;
    public UnityEvent<float> OnGlitchIntensityChanged;
    public UnityEvent<float> OnDamageTaken;  // Passes damage amount
    public UnityEvent OnPlayerDeath;
    
    private static PlayerHealth instance;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentHealth = maxHealth;
        currentOxygen = maxOxygen;
        currentRadiation = 0f;
        currentToxicity = 0f;
        coldResistance = 0f;
    }
    
    void Update()
    {
        HandleGlitch();
        HandleInvincibility();
    }
    
    void HandleInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }
    
    void HandleGlitch()
    {
        // Decay glitch intensity over time
        if (glitchIntensity > 0)
        {
            glitchIntensity -= glitchDecayRate * Time.deltaTime;
            glitchIntensity = Mathf.Max(glitchIntensity, 0f);
            OnGlitchIntensityChanged?.Invoke(glitchIntensity);
        }
    }
    
    // Oxygen methods
    public void AddOxygen(float amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Min(currentOxygen, maxOxygen);
        OnOxygenChanged?.Invoke(currentOxygen);
    }
    
    public void DrainOxygen(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Max(currentOxygen, 0);
        OnOxygenChanged?.Invoke(currentOxygen);
    }
    
    // Radiation methods
    public void AddRadiation(float amount)
    {
        currentRadiation += amount;
        currentRadiation = Mathf.Min(currentRadiation, maxRadiation);
        OnRadiationChanged?.Invoke(currentRadiation);
    }
    
    public void ReduceRadiation(float amount)
    {
        currentRadiation -= amount;
        currentRadiation = Mathf.Max(currentRadiation, 0);
        OnRadiationChanged?.Invoke(currentRadiation);
    }
    
    // Toxicity methods
    public void AddToxicity(float amount)
    {
        currentToxicity += amount;
        currentToxicity = Mathf.Min(currentToxicity, maxToxicity);
        OnToxicityChanged?.Invoke(currentToxicity);
    }
    
    public void ReduceToxicity(float amount)
    {
        currentToxicity -= amount;
        currentToxicity = Mathf.Max(currentToxicity, 0);
        OnToxicityChanged?.Invoke(currentToxicity);
    }
    
    // Health methods
    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible || damage <= 0f) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damage);
        
        // Trigger invincibility frames
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        OnPlayerDeath?.Invoke();
        Debug.Log("Player has died!");
    }
    
    public void AddHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void ReduceHealth(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    // Cold resistance methods
    public void AddColdResistance(float amount)
    {
        coldResistance += amount;
        coldResistance = Mathf.Min(coldResistance, maxColdResistance);
        OnColdResistanceChanged?.Invoke(coldResistance);
    }
    
    // Energy core methods
    public void ObtainEnergyCore()
    {
        if (!hasEnergyCore)
        {
            hasEnergyCore = true;
            OnEnergyCoreObtained?.Invoke();
        }
    }
    
    public bool HasEnergyCore() => hasEnergyCore;
    
    // Glitch methods
    public void SetGlitchIntensity(float intensity)
    {
        glitchIntensity = Mathf.Clamp(intensity, 0, maxGlitchIntensity);
        OnGlitchIntensityChanged?.Invoke(glitchIntensity);
    }
    
    public void AddGlitchIntensity(float amount)
    {
        glitchIntensity += amount;
        glitchIntensity = Mathf.Min(glitchIntensity, maxGlitchIntensity);
        OnGlitchIntensityChanged?.Invoke(glitchIntensity);
    }
    
    // Getter methods
    public float GetCurrentOxygen() => currentOxygen;
    public float GetMaxOxygen() => maxOxygen;
    public float GetOxygenPercentage() => currentOxygen / maxOxygen;
    
    public float GetCurrentRadiation() => currentRadiation;
    public float GetMaxRadiation() => maxRadiation;
    public float GetRadiationPercentage() => currentRadiation / maxRadiation;
    
    public float GetCurrentToxicity() => currentToxicity;
    public float GetMaxToxicity() => maxToxicity;
    public float GetToxicityPercentage() => currentToxicity / maxToxicity;
    
    public float GetGlitchIntensity() => glitchIntensity;
    public float GetMaxGlitchIntensity() => maxGlitchIntensity;
    
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    
    public float GetColdResistance() => coldResistance;
    public float GetMaxColdResistance() => maxColdResistance;
    public float GetColdResistancePercentage() => coldResistance / maxColdResistance;
    
    public bool IsDead() => isDead;
    public bool IsInvincible() => isInvincible;
    
    public static PlayerHealth Instance => instance;
}

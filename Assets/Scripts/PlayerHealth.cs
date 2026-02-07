using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Oxygen Settings")]
    [SerializeField] private float baseMaxOxygen = 100f;
    [SerializeField] private float currentOxygen;
    [SerializeField] private float baseOxygenDrainRate = 5f; // Base oxygen drain per second
    [SerializeField] private float oxygenRechargeRate = 10f; // Oxygen per second when not active
    [SerializeField] private float upgradedMaxOxygen = 150f; // Max oxygen after upgrade
    [SerializeField] private float upgradedOxygenDrainRate = 3f; // Drain rate after upgrade
    
    // Upgrade state
    private bool hasHelmetUpgrade = false;
    private float maxOxygen;
    private float oxygenDrainRate;
    
    [Header("Radiation Settings")]
    [SerializeField] private float maxRadiation = 100f;
    [SerializeField] private float currentRadiation = 0f;
    [SerializeField] private float radiationDamageThreshold = 50f; // Start taking damage at this radiation
    [SerializeField] private float baseRadiationDamagePerSecond = 5f; // Base damage per second above threshold
    [SerializeField] private float baseRadiationDecayRate = 2f; // Base radiation reduction per second
    [SerializeField] private float upgradedRadiationDamagePerSecond = 2f; // Damage after upgrade
    [SerializeField] private float upgradedRadiationDecayRate = 4f; // Decay rate after upgrade
    
    // Upgrade state
    private bool hasRadiationProtection = false;
    private float radiationDamagePerSecond;
    private float radiationDecayRate;
    
    [Header("Critical State Settings")]
    [SerializeField] private float healthDrainRateNoOxygen = 10f; // Health per second when oxygen is 0
    [SerializeField] private float healthDrainRateFullRadiation = 15f; // Health per second when radiation is full
    
    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 2f;
    
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnOxygenChanged;
    public UnityEvent<float> OnRadiationChanged;
    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnDeath;
    public UnityEvent OnHelmetUpgraded;
    public UnityEvent OnRadiationProtectionUpgraded;
    
    private bool isDead = false;
    private float totalDamageTaken = 0f;
    
    void Start()
    {
        currentHealth = maxHealth;
        CalculateUpgradedStats();
        currentOxygen = maxOxygen;
        currentRadiation = 0f;
    }
    
    void CalculateUpgradedStats()
    {
        // Set oxygen stats based on upgrade
        maxOxygen = hasHelmetUpgrade ? upgradedMaxOxygen : baseMaxOxygen;
        oxygenDrainRate = hasHelmetUpgrade ? upgradedOxygenDrainRate : baseOxygenDrainRate;
        
        // Set radiation stats based on upgrade
        radiationDamagePerSecond = hasRadiationProtection ? upgradedRadiationDamagePerSecond : baseRadiationDamagePerSecond;
        radiationDecayRate = hasRadiationProtection ? upgradedRadiationDecayRate : baseRadiationDecayRate;
    }
    
    void Update()
    {
        HandleOxygen();
        HandleRadiation();
        HandleCriticalStates();
    }
    
    void HandleOxygen()
    {
        // Recharge oxygen when not in use
        if (currentOxygen < maxOxygen)
        {
            currentOxygen += oxygenRechargeRate * Time.deltaTime;
            currentOxygen = Mathf.Min(currentOxygen, maxOxygen);
            OnOxygenChanged?.Invoke(currentOxygen);
        }
    }
    
    void HandleRadiation()
    {
        // Decay radiation over time
        if (currentRadiation > 0)
        {
            currentRadiation -= radiationDecayRate * Time.deltaTime;
            currentRadiation = Mathf.Max(currentRadiation, 0f);
            OnRadiationChanged?.Invoke(currentRadiation);
        }
        
        // Take damage if radiation is above threshold
        if (currentRadiation > radiationDamageThreshold)
        {
            float damageThisFrame = radiationDamagePerSecond * Time.deltaTime;
            TakeDamage(damageThisFrame);
        }
    }
    
    void HandleCriticalStates()
    {
        // Drain health if oxygen is depleted
        if (currentOxygen <= 0)
        {
            float damageThisFrame = healthDrainRateNoOxygen * Time.deltaTime;
            TakeDamage(damageThisFrame);
            Debug.Log("Health draining due to no oxygen! Health: " + currentHealth);
        }
        
        // Drain health if radiation is at maximum
        if (currentRadiation >= maxRadiation)
        {
            float damageThisFrame = healthDrainRateFullRadiation * Time.deltaTime;
            TakeDamage(damageThisFrame);
            Debug.Log("Health draining due to critical radiation! Health: " + currentHealth);
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        totalDamageTaken += damage;
        
        Debug.Log("Player took " + damage.ToString("F1") + " damage. Health: " + currentHealth + "/" + maxHealth + " | Total damage taken: " + totalDamageTaken.ToString("F1"));
        
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damage);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        Debug.Log("Player healed " + amount + ". Health: " + currentHealth + "/" + maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void DrainOxygen(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Max(currentOxygen, 0);
        
        Debug.Log("Oxygen drained " + amount + ". Oxygen: " + currentOxygen + "/" + maxOxygen);
        
        OnOxygenChanged?.Invoke(currentOxygen);
        
        // Take damage if oxygen depleted
        if (currentOxygen <= 0)
        {
            TakeDamage(10f); // Damage for suffocation
        }
    }
    
    public void AddRadiation(float amount)
    {
        currentRadiation += amount;
        currentRadiation = Mathf.Min(currentRadiation, maxRadiation);
        
        Debug.Log("Radiation increased by " + amount + ". Radiation: " + currentRadiation + "/" + maxRadiation);
        
        OnRadiationChanged?.Invoke(currentRadiation);
    }
    
    public void ReduceRadiation(float amount)
    {
        currentRadiation -= amount;
        currentRadiation = Mathf.Max(currentRadiation, 0);
        
        Debug.Log("Radiation reduced by " + amount + ". Radiation: " + currentRadiation + "/" + maxRadiation);
        
        OnRadiationChanged?.Invoke(currentRadiation);
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player died! Total damage taken: " + totalDamageTaken.ToString("F1"));
        
        OnDeath?.Invoke();
        
        // Disable components
        DisableComponents();
        
        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
    
    void DisableComponents()
    {
        // Disable colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        
        // Disable rigidbody if exists
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        // Disable player scripts
        Combat combat = GetComponent<Combat>();
        if (combat != null)
        {
            combat.enabled = false;
        }
    }
    
    // Getter methods
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    
    public float GetCurrentOxygen() => currentOxygen;
    public float GetMaxOxygen() => maxOxygen;
    public float GetOxygenPercentage() => currentOxygen / maxOxygen;
    
    public float GetCurrentRadiation() => currentRadiation;
    public float GetMaxRadiation() => maxRadiation;
    public float GetRadiationPercentage() => currentRadiation / maxRadiation;
    
    public float GetTotalDamageTaken() => totalDamageTaken;
    public bool IsDead() => isDead;
    
    // Upgrade methods
    public bool UpgradeHelmet()
    {
        if (hasHelmetUpgrade)
        {
            Debug.Log("Helmet already upgraded!");
            return false;
        }
        
        hasHelmetUpgrade = true;
        CalculateUpgradedStats();
        
        // Refill oxygen on upgrade
        currentOxygen = maxOxygen;
        
        Debug.Log("Helmet upgraded! Max oxygen: " + maxOxygen + ", Drain rate: " + oxygenDrainRate);
        OnHelmetUpgraded?.Invoke();
        OnOxygenChanged?.Invoke(currentOxygen);
        
        return true;
    }
    
    public bool UpgradeRadiationProtection()
    {
        if (hasRadiationProtection)
        {
            Debug.Log("Radiation protection already upgraded!");
            return false;
        }
        
        hasRadiationProtection = true;
        CalculateUpgradedStats();
        
        Debug.Log("Radiation protection upgraded! Damage per second: " + radiationDamagePerSecond + ", Decay rate: " + radiationDecayRate);
        OnRadiationProtectionUpgraded?.Invoke();
        
        return true;
    }
    
    // Upgrade getters
    public bool HasHelmetUpgrade() => hasHelmetUpgrade;
    public bool HasRadiationProtection() => hasRadiationProtection;
    
    public float GetCurrentOxygenDrainRate() => oxygenDrainRate;
    public float GetCurrentRadiationDamageRate() => radiationDamagePerSecond;
    public float GetCurrentRadiationDecayRate() => radiationDecayRate;
}

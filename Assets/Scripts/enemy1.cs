using UnityEngine;
using System.Collections;

public class enemy1 : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Attacking,
        Shielding,
        Dead
    }

    [Header("Targeting")]
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 12f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField, Range(0f, 1f)] private float shieldDamageReduction = 0.7f;

    [Header("Laser Attack")]
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private LaserAttack laserPrefab;
    [SerializeField] private float laserRange = 20f;
    [SerializeField] private int laserDamage = 15;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private LayerMask laserHitMask;

    [Header("Shield")]
    [SerializeField] private float shieldDuration = 2.5f;
    [SerializeField] private float shieldCooldown = 6f;
    [SerializeField] private GameObject shieldVisual;

    [Header("Reward")]
    [SerializeField] private RewardPickup rewardPrefab;
    [SerializeField] private int rewardAmount = 10;

    private EnemyState currentState = EnemyState.Idle;
    private int currentHealth;
    private float attackTimer;
    private float shieldTimer;
    private float shieldActiveTimer;
    private bool shieldActive;

    private void Awake()
    {
        currentHealth = maxHealth;
        attackTimer = attackCooldown;
        shieldTimer = shieldCooldown;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }

        UpdateTimers();
        AcquireTargetIfNeeded();

        if (shieldActive)
        {
            HandleShieldActive();
            return;
        }

        if (shieldTimer <= 0f)
        {
            ActivateShield();
            return;
        }

        if (target != null && attackTimer <= 0f)
        {
            StartCoroutine(FireLaser());
            attackTimer = attackCooldown;
            return;
        }

        SetState(target != null ? EnemyState.Idle : EnemyState.Idle);
    }

    public void TakeDamage(int amount)
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }

        int finalDamage = amount;
        if (shieldActive)
        {
            finalDamage = Mathf.CeilToInt(amount * (1f - shieldDamageReduction));
        }

        currentHealth -= finalDamage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateTimers()
    {
        attackTimer -= Time.deltaTime;
        shieldTimer -= Time.deltaTime;
    }

    private void AcquireTargetIfNeeded()
    {
        if (target != null)
        {
            return;
        }

        if (detectionRange <= 0f)
        {
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= detectionRange)
            {
                target = player.transform;
            }
        }
    }

    private void HandleShieldActive()
    {
        shieldActiveTimer -= Time.deltaTime;
        if (shieldActiveTimer <= 0f)
        {
            DeactivateShield();
        }
        else
        {
            SetState(EnemyState.Shielding);
        }
    }

    private void ActivateShield()
    {
        shieldActive = true;
        shieldActiveTimer = shieldDuration;
        shieldTimer = shieldCooldown;
        SetState(EnemyState.Shielding);
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }
    }

    private void DeactivateShield()
    {
        shieldActive = false;
        SetState(EnemyState.Idle);
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    private IEnumerator FireLaser()
    {
        SetState(EnemyState.Attacking);

        if (laserPrefab != null)
        {
            LaserAttack laserInstance = Instantiate(laserPrefab, laserOrigin != null ? laserOrigin.position : transform.position, Quaternion.identity);
            Vector3 origin = laserOrigin != null ? laserOrigin.position : transform.position;
            Vector3 direction = target != null ? (target.position - origin).normalized : transform.forward;
            laserInstance.Fire(origin, direction, laserRange, laserDamage, laserHitMask);
        }

        yield return null;
        SetState(EnemyState.Idle);
    }

    private void Die()
    {
        currentState = EnemyState.Dead;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }

        if (rewardPrefab != null)
        {
            RewardPickup rewardInstance = Instantiate(rewardPrefab, transform.position, Quaternion.identity);
            rewardInstance.SetAmount(rewardAmount);
        }

        gameObject.SetActive(false);
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }

        currentState = newState;
    }
}

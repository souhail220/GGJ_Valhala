using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Attack")]
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    public float attackDamage = 25f;

    [Header("Shield")]
    public float blockChance = 0.25f;   // 25% chance to block
    public float blockDuration = 1.5f;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask playerLayer;

    private Animator animator;
    private BossWalking walkingScript;
    private BossHealth bossHealthScript;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool isBlocking = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        walkingScript = GetComponent<BossWalking>();
        bossHealthScript = GetComponent<BossHealth>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // If player in attack range
        if (distance <= attackRange)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (isAttacking) return;
        animator.SetTrigger("Attack");
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        // Random chance to block instead
        if (Random.value < blockChance)
        {
            StartCoroutine(BlockRoutine());
        }
        else
        {
            StartCoroutine(AttackRoutine());
        }
    }

    System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // Stop movement
        if (walkingScript != null)
            walkingScript.StopBoss();

        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1.2f); // attack animation time

        animator.SetBool("IsAttacking", false);

        // Allow movement again
        if (walkingScript != null)
            walkingScript.ResumeBoss();

        isAttacking = false;
    }

    System.Collections.IEnumerator BlockRoutine()
    {
        isBlocking = true;
        animator.SetBool("IsBlocking", true);

        yield return new WaitForSeconds(blockDuration);

        animator.SetBool("IsBlocking", false);
        isBlocking = false;
    }

    // Called from animation event at hit frame
    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            PlayerHealth hp = hit.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(attackDamage);
        }
    }

    public void SetDead()
    {
        isDead = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

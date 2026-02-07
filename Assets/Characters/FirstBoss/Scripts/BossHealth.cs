using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    private BossWalking bossWalkingScript;
    public GameObject player;

    private Animator animator;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        bossWalkingScript = player.GetComponent<BossWalking>();
        if (bossWalkingScript == null)
            Debug.Log("script is null");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            Debug.Log("Boss got hit");
            animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        isDead = true;
        // Stop player movement
        if (bossWalkingScript != null)
        {
            Debug.Log("Boss died");
            bossWalkingScript.StopBoss();
        }
        else
        {
            Debug.Log("Boss not died");
        }
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(6f);
        Destroy(gameObject);
    }


    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(20f);
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    [Header("Defaults (can be overridden at spawn)")]
    [SerializeField] private float defaultSpeed = 100f;
    [SerializeField] private float defaultLifetime = 10f;
    [SerializeField] private float defaultDamage = 20f;
    [SerializeField] private LayerMask defaultHitLayers = ~0;
    [SerializeField] private GameObject defaultImpactEffect;

    [Header("Impact")]
    [SerializeField] private float impactForce = 10f;

    private Rigidbody rb;
    private float speed;
    private float lifetime;
    private float damage;
    private LayerMask hitLayers;
    private GameObject impactEffect;
    private bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        // Use ContinuousDynamic for fast projectiles to reduce tunneling risk
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Defaults in case Initialize isn't called
        speed = defaultSpeed;
        lifetime = defaultLifetime;
        damage = defaultDamage;
        hitLayers = defaultHitLayers;
        impactEffect = defaultImpactEffect;
    }

    // Call right after Instantiate to configure the projectile
    public void Initialize(float newSpeed, float newDamage, float newLifetime, LayerMask newHitLayers, GameObject newImpactEffect = null)
    {
        speed = newSpeed;
        damage = newDamage;
        lifetime = newLifetime;
        hitLayers = newHitLayers;
        if (newImpactEffect != null) impactEffect = newImpactEffect;

        // Set initial velocity immediately
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifetime);
        initialized = true;
    }

    private void Start()
    {
        // If not initialized externally, ensure velocity / lifetime are applied
        if (!initialized && rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, lifetime);
            initialized = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only react to configured layers
        if (((1 << collision.gameObject.layer) & hitLayers) == 0) return;

        var contact = collision.contacts[0];
        var otherCollider = collision.collider;

        // Apply damage if target has Health
        var health = otherCollider.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Apply impact force to hit rigidbody if present
        var targetRb = otherCollider.attachedRigidbody;
        if (targetRb != null)
        {
            targetRb.AddForceAtPosition(transform.forward * impactForce, contact.point, ForceMode.Impulse);
        }

        // Spawn impact VFX aligned with contact normal
        if (impactEffect != null)
        {
            var fx = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(fx, 3f);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Support trigger-based colliders as well
        if (((1 << other.gameObject.layer) & hitLayers) == 0) return;

        // Try apply damage
        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Apply force if target has Rigidbody
        var targetRb = other.attachedRigidbody;
        if (targetRb != null)
        {
            targetRb.AddForce(transform.forward * impactForce, ForceMode.Impulse);
        }

        if (impactEffect != null)
        {
            // Use forward as approximation for normal when trigger has no contact normal
            var fx = Instantiate(impactEffect, other.ClosestPoint(transform.position), Quaternion.LookRotation(transform.forward));
            Destroy(fx, 3f);
        }

        Destroy(gameObject);
    }
}
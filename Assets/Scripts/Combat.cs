using UnityEngine;
using UnityEngine.InputSystem;

public class Combat : MonoBehaviour
{
    [Header("Laser Weapon Settings")]
    [SerializeField] private float laserDamage = 20f;
    [SerializeField] private float fireRate = 0.2f; // Time between shots
    [SerializeField] private float laserRange = 100f;
    [SerializeField] private float laserDuration = 0.1f; // How long laser beam is visible
    
    [Header("Laser Visual")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool followCameraRotation = false;
    [SerializeField] private LayerMask hitLayers;
    
    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject muzzleFlashPrefab;
    
    // State
    private float lastFireTime = 0f;
    private GameObject currentLaser = null;
    
    // Input System
    private PlayerInput playerInput;
    private InputAction attackAction;
    
    void Start()
    {
        // Initialize fire point if not set
        if (firePoint == null)
        {
            firePoint = transform;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        // Set up Input System
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            attackAction = playerInput.actions["Attack"];
        }
        else
        {
            Debug.LogWarning("PlayerInput component not found on " + gameObject.name);
        }
    }
    
    void Update()
    {
        // Check for fire input using new Input System
        if (attackAction != null && attackAction.IsPressed() && Time.time - lastFireTime >= fireRate)
        {
            FireLaser();
        }
    }

    void LateUpdate()
    {
        if (followCameraRotation && firePoint != null && cameraTransform != null)
        {
            // Fire point is a child of the player, so apply only camera pitch in local space
            float pitch = cameraTransform.localEulerAngles.x;
            firePoint.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
    
    void FireLaser()
    {
        lastFireTime = Time.time;
        
        Debug.Log("Laser fired!");
        
        // Aim from camera (crosshair) but fire from gun to keep the beam straight out of the muzzle
        Transform aimTransform = cameraTransform != null ? cameraTransform : firePoint;
        Vector3 aimOrigin = aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetPoint = aimOrigin + aimDirection * laserRange;
        RaycastHit aimHit;

        if (Physics.Raycast(aimOrigin, aimDirection, out aimHit, laserRange, hitLayers))
        {
            targetPoint = aimHit.point;
        }

        Vector3 muzzleToTarget = (targetPoint - firePoint.position).normalized;
        float muzzleDistance = Vector3.Distance(firePoint.position, targetPoint);
        RaycastHit muzzleHit;

        if (Physics.Raycast(firePoint.position, muzzleToTarget, out muzzleHit, muzzleDistance, hitLayers))
        {
            // Muzzle hit takes priority (e.g., wall in front of the gun)
            Debug.Log("Laser hit " + muzzleHit.collider.name + " at distance " + muzzleHit.distance);

            Health health = muzzleHit.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(laserDamage);
            }

            SpawnLaserEffect(firePoint.position, muzzleHit.point);

            if (hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, muzzleHit.point, Quaternion.LookRotation(muzzleHit.normal));
                Destroy(hitEffect, 2f);
            }
        }
        else
        {
            // No obstruction from the gun, draw to the aim point
            SpawnLaserEffect(firePoint.position, targetPoint);
        }
        
        // Spawn muzzle flash
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(muzzleFlash, 1f);
        }
    }
    
    void SpawnLaserEffect(Vector3 start, Vector3 end)
    {
        if (laserPrefab == null)
        {
            Debug.LogWarning("Laser prefab not assigned! Assign a laser from Hovl Studio in the Inspector.");
            return;
        }
        
        // Destroy previous laser if it exists
        if (currentLaser != null)
        {
            Destroy(currentLaser);
        }
        
        // Spawn laser at fire point
        currentLaser = Instantiate(laserPrefab, start, firePoint.rotation);
        
        // Rotate laser to point toward the hit point
        Vector3 direction = (end - start).normalized;
        currentLaser.transform.forward = direction;
        
        // Destroy laser after duration
        Destroy(currentLaser, laserDuration);
    }
    
    // Visualize laser range in editor
    private void OnDrawGizmosSelected()
    {
        if (firePoint == null)
            firePoint = transform;
            
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePoint.position, firePoint.forward * laserRange);
    }
}

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserAttack : MonoBehaviour
{
    [SerializeField] private float beamDuration = 0.25f;
    [SerializeField] private float beamWidth = 0.1f;
    [SerializeField] private Color beamColor = Color.red;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = beamColor;
        lineRenderer.endColor = beamColor;
    }

    public void Fire(Vector3 origin, Vector3 direction, float range, int damage, LayerMask hitMask)
    {
        StartCoroutine(FireRoutine(origin, direction, range, damage, hitMask));
    }

    private IEnumerator FireRoutine(Vector3 origin, Vector3 direction, float range, int damage, LayerMask hitMask)
    {
        Vector3 endPosition = origin + direction * range;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, hitMask))
        {
            endPosition = hit.point;
            HitTarget(hit.collider.gameObject, damage);
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(beamDuration);

        lineRenderer.enabled = false;
        Destroy(gameObject);
    }

    private void HitTarget(GameObject targetObject, int damage)
    {
        IDamageable damageable = targetObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
}

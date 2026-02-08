using UnityEngine;

public class shooter : MonoBehaviour
{
    void FireLaser(Vector3 origin, Vector3 direction, float maxDistance)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            var target = hit.collider.GetComponent<laserCollision>();
            //if (target != null)
                //target.ApplyLaserHit();
        }
    }
}

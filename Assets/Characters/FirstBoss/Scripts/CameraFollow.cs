using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform target; // CameraHolder or head
    public float positionSmooth = 15f;
    public float rotationSmooth = 15f;

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth position
        transform.position = Vector3.Lerp(
            transform.position,
            target.position,
            positionSmooth * Time.deltaTime
        );

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target.rotation,
            rotationSmooth * Time.deltaTime
        );
    }
}

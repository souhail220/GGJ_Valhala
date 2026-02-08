using UnityEngine;

public class laserCollision : MonoBehaviour
{
    public Rigidbody rb;
    private void OnTriggerEnter(Collider other)
    {
        rb = GetComponent<Rigidbody>();
        // Default locked until hit
        

        if (other.gameObject.tag == "sphere")
        {
            Debug.Log("Hit sphere, unlocking physics.");
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}

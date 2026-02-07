using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RewardPickup : MonoBehaviour
{
    [SerializeField] private int rewardAmount = 10;
    [SerializeField] private float rotateSpeed = 45f;

    private void Awake()
    {
        Collider rewardCollider = GetComponent<Collider>();
        rewardCollider.isTrigger = true;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }

    public void SetAmount(int amount)
    {
        rewardAmount = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        RewardReceiver receiver = other.GetComponent<RewardReceiver>();
        if (receiver != null)
        {
            receiver.AddReward(rewardAmount);
            Destroy(gameObject);
        }
    }
}

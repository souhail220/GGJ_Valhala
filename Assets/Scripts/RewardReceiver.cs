using UnityEngine;

public class RewardReceiver : MonoBehaviour
{
    [SerializeField] private int currentReward;

    public int CurrentReward => currentReward;

    public void AddReward(int amount)
    {
        currentReward += amount;
    }
}

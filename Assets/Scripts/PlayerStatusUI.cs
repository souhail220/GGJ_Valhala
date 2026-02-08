using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Images (Set Image Type to Filled)")]
    [SerializeField] private Image healthImage;
    [SerializeField] private Image oxygenImage;
    [SerializeField] private Image toxicityImage;
    [SerializeField] private Image radiationImage;
    [SerializeField] private Image coldImage;

    private PlayerHealth playerHealth;

    private void Update()
    {
        if (playerHealth == null)
        {
            playerHealth = PlayerHealth.Instance;
            if (playerHealth == null)
            {
                playerHealth = FindAnyObjectByType<PlayerHealth>();
            }
            if (playerHealth == null) return;
        }

        if (healthImage != null)
            healthImage.fillAmount = playerHealth.GetHealthPercentage();

        if (oxygenImage != null)
            oxygenImage.fillAmount = playerHealth.GetOxygenPercentage();

        if (toxicityImage != null)
            toxicityImage.fillAmount = playerHealth.GetToxicityPercentage();

        if (radiationImage != null)
            radiationImage.fillAmount = playerHealth.GetRadiationPercentage();

        if (coldImage != null)
            coldImage.fillAmount = playerHealth.GetColdResistancePercentage();
    }
}

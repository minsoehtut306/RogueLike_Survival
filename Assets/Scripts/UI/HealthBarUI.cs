using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fillImage; // Image Type = Filled

    void Update()
    {
        if (!playerHealth || !fillImage) return;

        float t = (playerHealth.maxHP <= 0) ? 0f : (float)playerHealth.currentHP / playerHealth.maxHP;
        t = Mathf.Clamp01(t);

        fillImage.fillAmount = t;

        Debug.Log($"[HealthBarUI] hp={playerHealth.currentHP}/{playerHealth.maxHP} fill={t} image={fillImage.name}");
    }


}

using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    // Inspector: References
    [Header("References")]
    [Tooltip("PlayerHealth component to read HP from.")]
    public PlayerHealth playerHealth;

    [Tooltip("UI Image used as the fill (Image Type should be Filled).")]
    public Image fillImage;

    // Unity lifecycle
    private void Awake()
    {
        // Auto-find PlayerHealth if not assigned
        if (playerHealth == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerHealth == null || fillImage == null) return;

        // Avoid divide-by-zero (if maxHP ever becomes 0 due to setup mistake)
        float max = Mathf.Max(1, playerHealth.maxHP);
        float fill = playerHealth.currentHP / max;

        // Clamp so the UI never goes below 0 or above 1
        fillImage.fillAmount = Mathf.Clamp01(fill);
    }
}

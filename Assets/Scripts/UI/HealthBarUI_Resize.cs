using UnityEngine;

public class HealthBarUI_Resize : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public RectTransform fillRect;

    float fullWidth;

    void Start()
    {
        if (fillRect != null)
            fullWidth = fillRect.sizeDelta.x;
    }

    void Update()
    {
        if (!playerHealth || !fillRect) return;

        float t = (playerHealth.maxHP <= 0) ? 0f : (float)playerHealth.currentHP / playerHealth.maxHP;
        t = Mathf.Clamp01(t);

        var size = fillRect.sizeDelta;
        size.x = fullWidth * t;
        fillRect.sizeDelta = size;
    }
}

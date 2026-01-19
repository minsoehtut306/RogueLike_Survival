using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    [Header("Invincibility")]
    public float invincibleTime = 0.35f;
    float invTimer;

    void Awake()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        if (invTimer > 0f) invTimer -= Time.deltaTime;
    }

    public bool IsInvincible => invTimer > 0f;

    public void TakeDamage(int amount)
    {
        if (IsInvincible) return;

        currentHP -= amount;
        invTimer = invincibleTime;

        if (currentHP <= 0)
        {
            currentHP = 0;
            GetComponent<Animator>()?.SetTrigger("Die");
        }
        else
        {
            Debug.Log($"Player HP: {currentHP}/{maxHP}");
        }
    }
}

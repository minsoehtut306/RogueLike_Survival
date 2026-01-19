using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 0.5f;

    float timer;

    void Update()
    {
        if (timer > 0f) timer -= Time.deltaTime;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    void TryDamage(Collider2D other)
    {
        if (timer > 0f) return;

        // Accept Player tag on child OR parent
        if (!other.CompareTag("Player") && (other.transform.parent == null || !other.transform.parent.CompareTag("Player")))
            return;

        // Find health even if collider is on a child
        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        health.TakeDamage(damage);
        timer = damageInterval;
    }

}

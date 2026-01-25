using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    // Inspector: Target
    [Header("Target")]
    [Tooltip("Transform the camera should follow (usually the Player).")]
    public Transform target;

    // Inspector: Follow behaviour
    [Header("Follow Settings")]
    [Tooltip("Smaller value = tighter follow, larger value = smoother follow.")]
    public float smoothTime = 0.08f;

    [Tooltip("Offset applied to the camera position.")]
    public Vector2 offset;

    // Internal state
    private Vector3 velocity; // used internally by SmoothDamp

    // Unity lifecycle
    private void Awake()
    {
        // Auto-find player if not assigned
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                target = p.transform;
        }
    }

    private void LateUpdate()
    {
        // LateUpdate ensures the camera moves after the player has moved
        if (target == null) return;

        // Desired camera position (keep Z unchanged)
        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        // Smoothly move camera toward desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            smoothTime
        );
    }
}

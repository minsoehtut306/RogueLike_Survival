using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Joystick (DynamicJoystick)")]
    public DynamicJoystick joystick; // drag TouchZone here

    Rigidbody2D rb;
    Animator anim;

    Vector2 moveInput;
    Vector2 lastDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        moveInput = ReadDynamicJoystick();

        float speed = moveInput.sqrMagnitude;

        if (speed > 0.001f)
        {
            lastDir = moveInput.normalized;
        }

        // Animator params (optional)
        if (anim != null)
        {
            anim.SetFloat("MoveX", lastDir.x);
            anim.SetFloat("MoveY", lastDir.y);
            anim.SetFloat("Speed", Mathf.Sqrt(speed));
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    Vector2 ReadDynamicJoystick()
    {
        if (joystick == null) return Vector2.zero;

        // Read JoystickKnob position relative to JoystickBase
        // This avoids depending on "Direction/Value/Horizontal/Vertical" API differences.
        var t = joystick.GetType();

        var baseField = t.GetField("joystickBase", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        var knobField = t.GetField("joystickKnob", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        var radiusField = t.GetField("maxRadius", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

        if (baseField == null || knobField == null) return Vector2.zero;

        RectTransform baseRT = baseField.GetValue(joystick) as RectTransform;
        RectTransform knobRT = knobField.GetValue(joystick) as RectTransform;

        float radius = 80f;
        if (radiusField != null)
        {
            object r = radiusField.GetValue(joystick);
            if (r is float f) radius = f;
            if (r is int i) radius = i;
        }

        if (baseRT == null || knobRT == null || radius <= 0.001f) return Vector2.zero;

        // anchoredPosition is local offset in UI space
        Vector2 knobOffset = knobRT.anchoredPosition;
        Vector2 input = knobOffset / radius;

        // clamp to unit circle
        if (input.magnitude > 1f) input = input.normalized;

        return input;
    }
}

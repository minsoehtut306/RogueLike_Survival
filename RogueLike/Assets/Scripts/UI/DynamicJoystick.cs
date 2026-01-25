using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // Inspector: References
    [Header("References")]
    [Tooltip("The joystick background/base (usually a UI Image RectTransform).")]
    public RectTransform joystickBase;

    [Tooltip("The joystick knob/handle (child UI Image RectTransform).")]
    public RectTransform joystickKnob;

    // Inspector: Settings
    [Header("Settings")]
    [Tooltip("Maximum distance (in pixels) the knob can move from the base center.")]
    public float maxRadius = 80f;

    // Internal state
    private Vector2 startPos;       // pointer position where touch started
    private Vector2 inputVector;    // normalised input (-1..1)

    // Public API
    // Returns movement input in range [-1..1] for both x and y.
    public Vector2 Input => inputVector;

    // Unity lifecycle
    private void Awake()
    {
        // Hide joystick until the player touches the screen
        if (joystickBase != null)
            joystickBase.gameObject.SetActive(false);
    }

    // Input events
    public void OnPointerDown(PointerEventData eventData)
    {
        if (joystickBase == null || joystickKnob == null) return;

        // Show joystick and place it where the user touched
        joystickBase.gameObject.SetActive(true);
        joystickBase.position = eventData.position;

        // Reset knob and input
        joystickKnob.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;

        // Save start position for calculating drag delta
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (joystickBase == null || joystickKnob == null) return;

        // Calculate drag delta from the initial touch point
        Vector2 delta = eventData.position - startPos;

        // Clamp to max radius so the knob stays inside the joystick base
        delta = Vector2.ClampMagnitude(delta, maxRadius);

        // Move knob visually
        joystickKnob.anchoredPosition = delta;

        // Convert delta into a normalised input vector
        // Example: half radius -> input ~0.5
        inputVector = delta / maxRadius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (joystickBase == null || joystickKnob == null) return;

        // Hide joystick and reset everything
        joystickBase.gameObject.SetActive(false);
        joystickKnob.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }
}

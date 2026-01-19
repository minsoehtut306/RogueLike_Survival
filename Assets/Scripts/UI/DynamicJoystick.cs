using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform joystickBase;
    public RectTransform joystickKnob;
    public float maxRadius = 80f;

    Vector2 startPos;
    Vector2 inputVector;

    public Vector2 Input => inputVector;

    void Awake()
    {
        joystickBase.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystickBase.gameObject.SetActive(true);

        joystickBase.position = eventData.position;
        joystickKnob.anchoredPosition = Vector2.zero;

        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - startPos;
        delta = Vector2.ClampMagnitude(delta, maxRadius);

        joystickKnob.anchoredPosition = delta;

        inputVector = delta / maxRadius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickBase.gameObject.SetActive(false);
        joystickKnob.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }
}

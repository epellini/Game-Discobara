using UnityEngine;
using UnityEngine.EventSystems; // Required for UI Event handling

public class ButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 clickedScale = new Vector3(0.9f, 0.9f, 0.9f); // Scale of the button when clicked
    private Vector3 originalScale; // Original scale to return to on release

    void Start()
    {
        // Store the original scale
        originalScale = transform.localScale;
    }

    // Called when the pointer is down on the object
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = clickedScale; // Scale down the button
    }

    // Called when the pointer is released
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale; // Return to original scale
    }
}

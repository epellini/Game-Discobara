using System.Collections;
using UnityEngine;

public class CameraBounceZoom : MonoBehaviour
{
    private Camera cam;
    public Light partyLight;
    private float originalLightIntensity;
    private Vector3 originalPosition;
    private float originalSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        originalPosition = transform.localPosition;
        originalSize = cam.orthographicSize;
        if (partyLight != null)
        {
            originalLightIntensity = partyLight.intensity;
            partyLight.enabled = false;
        }
    }

    public void BounceAndZoomCamera()
    {
        StartCoroutine(BounceAndZoom(8f, 0.2f, 0.15f));
    }

    public IEnumerator BounceAndZoom(float duration, float bounceMagnitude, float zoomMagnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float originalSize = cam.orthographicSize;

        // Enable and adjust light at the start of the effect
        if (partyLight != null)
        {
            partyLight.enabled = true;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Bounce effect using a sine wave
            float y = Mathf.Sin(elapsed * Mathf.PI * 2) * bounceMagnitude;
            transform.localPosition = new Vector3(originalPos.x, originalPos.y + y, originalPos.z);

            // Zoom effect
            cam.orthographicSize = originalSize + Mathf.Sin(elapsed * Mathf.PI * 2) * zoomMagnitude;

            // Adjust light intensity
            if (partyLight != null)
            {
                partyLight.intensity = originalLightIntensity + Mathf.Sin(elapsed * Mathf.PI * 2) * zoomMagnitude;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        ResetEffects();
    }

    public IEnumerator DeathShake(float duration, float bounceMagnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Bounce effect using a sine wave
            float x = Random.Range(-1f, 1f) * bounceMagnitude;
            float y = Random.Range(-1f, 1f) * bounceMagnitude;
            // Apply shake effect relative to the original position
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public void ResetEffects()
    {
        transform.localPosition = originalPosition;
        cam.orthographicSize = originalSize;
        if (partyLight != null)
        {
            partyLight.intensity = originalLightIntensity;
            partyLight.enabled = false;
        }
    }
}

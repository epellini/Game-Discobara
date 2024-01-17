using UnityEngine;
using TMPro;
using System.Collections;
public class EffectCountdown : MonoBehaviour
{
    public TextMeshProUGUI slowMotionCountdownText; 
    public TextMeshProUGUI extraPointsCountdownText; 
    private float slowMotionDuration = 5f; // Duration of the countdown in seconds
    private float extraPointsDuration = 7f; // Duration of the countdown in seconds

    void Start()
    {
        HideSlowMotionCountdown();
        HideExtraPointsCountdown();
    }

    public void StartSlowMotionCountdown()
    {
        StartCoroutine(SlowMotionCoroutine());
    }

    public void StartExtraPointsCountdown()
    {
        StartCoroutine(ExtraPointsCoroutine());
    }

    // Slow motion countdown
    private IEnumerator SlowMotionCoroutine()
    {
        float remainingTime = slowMotionDuration;
        while (remainingTime > 0)
        {
            slowMotionCountdownText.text = remainingTime.ToString("F0"); // F0 means no decimal places
            yield return new WaitForSecondsRealtime(0.1f);
            remainingTime -= 0.1f;
        }
        HideSlowMotionCountdown();
    }

    // Extra points countdown
    private IEnumerator ExtraPointsCoroutine()
    {
        float remainingTime = extraPointsDuration;
        while (remainingTime > 0)
        {
            extraPointsCountdownText.text = remainingTime.ToString("F0"); // F0 means no decimal places
            yield return new WaitForSecondsRealtime(0.1f);
            remainingTime -= 0.1f;
        }
        HideExtraPointsCountdown();
    }

    private void HideSlowMotionCountdown()
    {
        slowMotionCountdownText.text = "";
    }
    private void HideExtraPointsCountdown()
    {
        extraPointsCountdownText.text = "";
    }
}

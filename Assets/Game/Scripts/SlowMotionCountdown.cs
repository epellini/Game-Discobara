using UnityEngine;
using TMPro;
using System.Collections;

public class SlowMotionCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Assign this in the inspector
    private float countdownDuration = 5f; // Duration of the countdown in seconds

    void Start()
    {
        HideCountdown();
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        float remainingTime = countdownDuration;
        while (remainingTime > 0)
        {
            countdownText.text = remainingTime.ToString("F0"); // F0 means no decimal places
            yield return new WaitForSecondsRealtime(0.1f);
            remainingTime -= 0.1f;
        }
        HideCountdown();
    }

    private void HideCountdown()
    {
        countdownText.text = "";
    }
}

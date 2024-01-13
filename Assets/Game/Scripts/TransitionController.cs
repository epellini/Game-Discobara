using System.Collections;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    private Animator animator;
    private static readonly int StartTrigger = Animator.StringToHash("Start");

    void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false); 
    }

    public void StartFadeOut()
    {
        gameObject.SetActive(true);
        animator.SetTrigger(StartTrigger);
        StartCoroutine(DeactivateAfterAnimation());
    }

    private IEnumerator DeactivateAfterAnimation()
    {
        // Wait for the animation duration (1 second)
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}

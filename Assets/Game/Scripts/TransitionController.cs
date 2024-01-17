using System.Collections;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false); 
    }

    public void PlayTransition()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Transition");
        StartCoroutine(DeactivateAfterAnimation());
    }

    private IEnumerator DeactivateAfterAnimation()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}

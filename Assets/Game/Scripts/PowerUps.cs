using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PowerUps : MonoBehaviour
{
    public static PowerUps Instance;
    public CameraBounceZoom cameraBounceZoom;
    [SerializeField] private Animator playerAnimator; // Assign this in the inspector
    [SerializeField] private GameManager gameManager; // Assign this in the inspector
    [SerializeField] public GameObject powerUpSlowMotionPrefab; // Assign this in the inspector
    [SerializeField] public GameObject powerUpExtraPointsPrefab; // Assign this in the inspector
    public float normalSpawnDelay = 1f; // Normal delay between platform spawns
    public float slowMotionSpawnDelay = 2f; // Delay during slow motion
    public bool isSlowMotionActive = false;
    //public float slowMotionSpawnChance = 0f; 
    public float extraPointsSpawnChance = 0.05f;
    public bool isExtraPointsActive = false;
     //[SerializeField] private Transform playerHeadTransform; // Assign the player's head transform in the inspector
    //[SerializeField] private GameObject effectObject; // Assign the effect prefab in the inspector
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //SetEffectVisibility(false);
    }

    public void ActivateExtraPoints()
    {
        StartCoroutine(cameraBounceZoom.BounceAndZoom(8f, 0.2f, 0.15f));
        playerAnimator.SetTrigger("Special");
        //SetEffectVisibility(true);
        isExtraPointsActive = true;
        StartCoroutine(ExtraPointsDuration());
    }

    private IEnumerator ExtraPointsDuration()
    {
        yield return new WaitForSeconds(8f); // Duration of the extra points
        isExtraPointsActive = false;
        playerAnimator.SetTrigger("Reset");
    }

    // public void ActivateSlowMotion()
    // {
    //     isSlowMotionActive = true;
    //     SetEffectVisibility(true);
    //     StartCoroutine(SlowMotionDuration());
    // }

    // private IEnumerator SlowMotionDuration()
    // {
    //     yield return new WaitForSeconds(5f); // Duration of the slow motion
    //     isSlowMotionActive = false;
    //     SetEffectVisibility(false);
    // }
    // private void SetEffectVisibility(bool isVisible)
    // {
    //     if (effectObject != null)
    //     {
    //         effectObject.SetActive(isVisible);
    //     }
    // }
}

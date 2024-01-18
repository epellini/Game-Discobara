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
    public float slowMotionSpawnChance = 3f; //  3% chance of spawning a power-up
    public float extraPointsSpawnChance = 6f; //
    public bool isExtraPointsActive = false;
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
    }

    public void ActivateExtraPoints()
    {
        StartCoroutine(cameraBounceZoom.BounceAndZoom(6f, 0.2f, 0.15f));
        playerAnimator.SetTrigger("Special");
        isExtraPointsActive = true;
        StartCoroutine(ExtraPointsDuration());
    }

    private IEnumerator ExtraPointsDuration()
    {
        yield return new WaitForSeconds(6f); // Duration of the extra points
        isExtraPointsActive = false;
        playerAnimator.SetTrigger("Reset");
    }

    public void ActivateSlowMotion()
    {
        isSlowMotionActive = true;
        StartCoroutine(SlowMotionDuration());
    }

    private IEnumerator SlowMotionDuration()
    {
        yield return new WaitForSeconds(5f); // Duration of the slow motion
        isSlowMotionActive = false;
    }
}

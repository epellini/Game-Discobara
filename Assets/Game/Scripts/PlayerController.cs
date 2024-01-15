using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    private bool hasFallen = false;
    public SlowMotionCountdown slowMotionCountdown; // Assign this in the inspector
    void Update()
    {
         if (hasFallen) 
        {
            GameManager.Instance.PlayerFell();
            
           //GetComponent<Rigidbody>().isKinematic = true;
            hasFallen = false; // Set the flag to true to prevent further calls
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            ScoreManager.Instance.IncrementScore();
        }

        if (collision.gameObject.CompareTag("Wrong"))
        {
            hasFallen = true;
        }
    }

    public void ResetPlayer()
    {
        hasFallen = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            GameManager.Instance.ActivateSlowMotion();
            slowMotionCountdown.StartCountdown();
            Destroy(other.gameObject); // Remove the power-up after pickup
        }
    }

}

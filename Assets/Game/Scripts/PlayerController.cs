using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    private bool hasFallen = false;
    void Update()
    {
         if (!hasFallen && transform.position.y < -0.5f)
        {
            GameManager.Instance.PlayerFell();
            GetComponent<Rigidbody>().isKinematic = true;
            hasFallen = true; // Set the flag to true to prevent further calls
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            ScoreManager.Instance.IncrementScore();
        }
    }

    public void ResetPlayer()
    {
        hasFallen = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}

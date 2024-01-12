using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        // Check the players Y position and call the PlayerFell method if they fall below -0.5
        if (transform.position.y < -0.5f)
        {
            GameManager.Instance.PlayerFell();
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            ScoreManager.Instance.IncrementScore();
        }
    }
}

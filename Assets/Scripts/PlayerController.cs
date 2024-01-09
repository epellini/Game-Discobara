using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Assuming "plane" is the tag assigned to your plane GameObject
        if (collision.gameObject.CompareTag("plane"))
        {
            Debug.Log("Player has fallen!");
            if (gameManager != null)
            {
                gameManager.PlayerFell();
            }
        }
    }
}

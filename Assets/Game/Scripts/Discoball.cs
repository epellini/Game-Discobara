using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discoball : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    void Update()
    {
        transform.Rotate (0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}

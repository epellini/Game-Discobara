using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] songSegments;
    private int currentSegmentIndex = 0;
 public void PlayNextSegment()
    {
        int nextSegmentIndex = Random.Range(0, songSegments.Length);
        audioSource.clip = songSegments[nextSegmentIndex];
        audioSource.Play();
    }
}

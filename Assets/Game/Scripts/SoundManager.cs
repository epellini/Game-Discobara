using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource _musicSource, _effectsSource;
    [SerializeField] public AudioClip[] audioClips;

    private void Awake()
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

    public void ButtonPress()
    {
        _effectsSource.PlayOneShot(audioClips[0]);
    }
    public void RightMove()
    {
        _effectsSource.PlayOneShot(audioClips[1]);
    }

    public void WrongMove()
    {
        _effectsSource.PlayOneShot(audioClips[2]);
    }
    
    public void Bite()
    {
        _effectsSource.PlayOneShot(audioClips[3]);
    }


}

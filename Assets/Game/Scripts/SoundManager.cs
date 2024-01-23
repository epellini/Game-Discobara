using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public List<AudioClip> musicTracks;
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

    public void Start()
    {
        PlayRandomSong();
    }

    public void PlayRandomSong()
    {
        if (musicTracks.Count > 0)
        {
            _musicSource.clip = musicTracks[Random.Range(0, musicTracks.Count)];
            _musicSource.Play();
        }
    }

    public void ButtonPress()
    {
        _effectsSource.PlayOneShot(audioClips[0]);
    }
    public void RightMove()
    {
        _effectsSource.pitch = Random.Range(0.9f, 1.1f);
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

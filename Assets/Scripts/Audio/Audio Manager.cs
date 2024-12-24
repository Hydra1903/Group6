using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    public AudioClip backgroundMusic;
    public AudioClip footSteps;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }
}

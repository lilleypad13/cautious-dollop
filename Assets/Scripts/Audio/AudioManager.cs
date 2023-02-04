using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public SoundClip[] musicsSounds;
    public SoundClip[] sfxSounds;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string audioName)
    {
        SoundClip sound = Array.Find(musicsSounds, x => x.audioName == audioName);

        if(sound == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string audioName)
    {
        SoundClip sound = Array.Find(sfxSounds, x => x.audioName == audioName);

        if (sound == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.PlayOneShot(sound.clip);
        }
    }
}

using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip battleMusic;

    [Header("Volume")]
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    private bool isMuted = false;
    private float lastMusicVolume = 1f;

    private bool isSFXMuted = false;
    private float lastSFXVolume = 1f;

    private bool isAllMuted = false;

    public event Action AudioStateChanged;
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

    public void PlayMainMenuMusic()
    {
        if (musicSource.clip == mainMenuMusic)
            return;

        musicSource.clip = mainMenuMusic;
        musicSource.Play();
    }

    public void PlayBattleMusic()
    {
        if (musicSource.clip == battleMusic)
            return;

        musicSource.clip = battleMusic;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;

        if (volume > 0)
        {
            lastMusicVolume = volume;
            isMuted = false;
        }
        else
        {
            isMuted = true;
        }

        AudioStateChanged?.Invoke();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;

        if (volume > 0)
        {
            lastSFXVolume = volume;
            isSFXMuted = false;
        }
        else
        {
            isSFXMuted = true;
        }

        AudioStateChanged?.Invoke();
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public bool IsMuted()
    {
        return isMuted;
    }

    public float GetLastMusicVolume()
    {
        return lastMusicVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public bool IsSFXMuted()
    {
        return isSFXMuted;
    }

    public float GetLastSFXVolume()
    {
        return lastSFXVolume;
    }

    public bool IsAllMuted()
    {
        return isAllMuted;
    }

    public void SetMuteAll(bool mute)
    {
        isAllMuted = mute;

        AudioListener.volume = mute ? 0f : 1f;

        Debug.Log(
            $"[AudioManager] SetMuteAll: {mute}, " +
            $"AudioListener volume: {AudioListener.volume}"
        );

        AudioStateChanged?.Invoke();
    }
    
}
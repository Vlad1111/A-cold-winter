using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicScript : MonoBehaviour
{
    public static BackgroundMusicScript instance = null;

    public AudioClip backgroundMusicClip;
    public float volumeMultiplyer = 0;
    private float lastVolume = 0;

    public AudioSource m1;
    public AudioSource m2;
    private int curentSource = 0;
    private float volume = 0;
    public float transitionSpeed = 1;

    private void Awake()
    {
        if (instance != null)
        {
            instance.playSound(backgroundMusicClip, volumeMultiplyer, transitionSpeed);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            m1.clip = backgroundMusicClip;
            m1.Play();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void playSound(AudioClip clip, float volumeMultiplyer, float transitionSpeed)
    {
        if (clip == null || clip == backgroundMusicClip)
            return;

        this.backgroundMusicClip = clip;
        this.transitionSpeed = transitionSpeed;
        curentSource = (curentSource + 1) % 2;

        lastVolume = volumeMultiplyer;

        if (curentSource == 0)
        {
            m1.clip = clip;
            m1.volume = 0;
            m1.Play();
        }
        else
        {
            m2.clip = clip;
            m2.volume = 0;
            m2.Play();
        }

        volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(curentSource == 0)
        {
            m1.volume = volume * volumeMultiplyer * MySettings.MusicVolume * MySettings.OverallVolumes;
            m2.volume = (1 - volume) * lastVolume * MySettings.MusicVolume * MySettings.OverallVolumes;
        }
        else
        {
            m2.volume = volume * volumeMultiplyer * MySettings.MusicVolume * MySettings.OverallVolumes;
            m1.volume = (1 - volume) * lastVolume * MySettings.MusicVolume * MySettings.OverallVolumes;
        }
        volume += Time.deltaTime / transitionSpeed;
        if (volume > 1)
            volume = 1;
    }
}

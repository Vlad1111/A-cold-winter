using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {

        public string name;

        public AudioClip clip;
        public AudioClip[] randomClips;

        [Range(0f, 3f)]
        public float volume = .75f;
        [Range(0f, 1f)]
        public float volumeVariance = .1f;

        [Range(.1f, 3f)]
        public float pitch = 1f;
        [Range(0f, 1f)]
        public float pitchVariance = .1f;

        public bool loop = false;

        public AudioMixerGroup mixerGroup;

        [HideInInspector]
        public AudioSource source;

    }

    //public static SoundManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    void Awake()
    {
        //if (instance != null)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //if(instance == null)
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume * MySettings.EffectsVolume * MySettings.OverallVolumes;
            s.source.pitch = s.pitch;
            s.source.outputAudioMixerGroup = mixerGroup;
            s.source.minDistance = s.source.maxDistance / 2;
        }
    }

    void Start()
    {
        //Play("AmbientForest");
        //Play("Crows");
    }

    public void Play(string sound, bool randomSound = false)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }
        s.source.Stop();

        if (randomSound)
        {
            if (s.randomClips.Length != 0)
                s.source.clip = s.randomClips[(int)(UnityEngine.Random.value * s.randomClips.Length)];
        }
        else
            s.source.clip = s.clip;

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f)) * MySettings.EffectsVolume * MySettings.OverallVolumes;
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Play();
    }

    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        s.source.Stop();
    }

    public void Pause(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }
        s.source.Pause();
        s.source.volume = s.volume * MySettings.EffectsVolume * MySettings.OverallVolumes;
    }

    public void UnPause(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        if (!s.source.isPlaying)
            s.source.Play();
        else
            s.source.UnPause();
        s.source.volume = s.volume * MySettings.EffectsVolume * MySettings.OverallVolumes;
    }

    public void PauseAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Pause();
        }
    }

    public void UnPauseAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.UnPause();
        }
    }

    public AudioSource GetSound(string sound) //Use only for spaghetty code
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return null;
        }

        return s.source;
    }

    public Sound GetObjectSound(string sound) //Use only for spaghetty code
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return null;
        }

        return s;
    }

}

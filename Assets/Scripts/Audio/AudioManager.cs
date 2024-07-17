using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Sound[] sounds;

    public Sound[] zombieGrowl;
    public Sound[] zombieAttack;
    public Sound[] zombieDie;

    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(AudioManager).Name);
                    _instance = singletonObject.AddComponent<AudioManager>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        InitSoundList(sounds);

        InitSoundList(zombieGrowl);
        InitSoundList(zombieAttack);
        InitSoundList(zombieDie);
    }

    private void InitSoundList(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            if (!s.createSource)
                continue;

            s.source = gameObject.AddComponent<AudioSource>();
            InitAudioSource(s.source, s);
        }
    }

    public void InitAudioSource(AudioSource source, Sound sound)
    {
        source.clip = sound.clip;
        source.outputAudioMixerGroup = sound.mixerGroup;
        source.pitch = sound.pitch;
        source.volume = sound.volume;
        source.loop = sound.loop;
    }

    public Sound FindSound(Sound.SoundName name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name.Equals(name))
                return s;
        }
        return null;
    }

    public void Play(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);
        ResetVolumeOfSound(s);
        s.source.Play();
    }

    public void PlayOneShot(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);
        ResetVolumeOfSound(s);
        s.source.PlayOneShot(s.clip);
    }

    public void OnlyPlayAfterSoundEnds(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);
        if (s.name.Equals(sound) && !s.source.isPlaying)
            s.source.Play();
    }

    public void Stop(Sound.SoundName sound)
    {
        FindSound(sound).source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.Stop();
    }

    public void Pause(Sound.SoundName sound)
    {
        FindSound(sound).source.Pause();
    }

    public void Unpause(Sound.SoundName sound)
    {
        FindSound(sound).source.UnPause();
    }

    public void PauseAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.Pause();
    }

    public void UnpauseAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.UnPause();
    }

    public bool CheckIfSoundPlaying(Sound.SoundName sound)
    {
        return FindSound(sound).source.isPlaying;
    }

    public void FadeAllSound(bool fadeIn, float duration, float targetVolume)
    {
        foreach (Sound s in sounds)
        {
            StopFadeRoutine(s.name);
            s.fadeRoutine = StartCoroutine(s.FadeSoundRoutine(fadeIn, duration, targetVolume));
        }
    }

    public void FadeSound(bool fadeIn, Sound.SoundName sound, float duration, float targetVolume)
    {
        Sound s = FindSound(sound);
        StopFadeRoutine(sound);
        s.fadeRoutine = StartCoroutine(s.FadeSoundRoutine(fadeIn, duration, targetVolume));
    }

    public void StopFadeRoutine(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);

        if (s.fadeRoutine != null)
        {
            StopCoroutine(s.fadeRoutine);
            s.fadeRoutine = null;
            ResetVolumeOfSound(s);
        }
    }

    private void ResetVolumeOfSound(Sound sound)
    {
        if (sound.source == null)
            return;

        FindSound(sound.name).source.volume = sound.volume;
    }

    public void PlayAfterDelay(float delay, Sound.SoundName sound)
    {
        FindSound(sound).source.PlayDelayed(delay);
    }

    public void SetPitch(Sound.SoundName sound, float newPitch)
    {
        Sound s = FindSound(sound);
        s.source.pitch = newPitch;
    }
}
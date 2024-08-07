using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class AudioPlayer : PooledObject
{
    [SerializeField] private AudioSource audioSource;

    public void SetupAudioPlayer(Sound soundToPlay)
    {
        AudioManager.Instance.PlayOneShot(audioSource, soundToPlay.name);
        Deactivate(soundToPlay.clip.length);
    }

    public void Deactivate(float lifetime)
    {
        StartCoroutine(DeactivateRoutine(lifetime));
    }

    private IEnumerator DeactivateRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        Release();
        gameObject.SetActive(false);
    }
}
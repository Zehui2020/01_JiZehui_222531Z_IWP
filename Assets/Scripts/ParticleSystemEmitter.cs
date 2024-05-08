using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemEmitter : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;

    public void PlayPS()
    {
        foreach (ParticleSystem ps in particleSystems)
            ps.Emit(1);
    }

    public void PlayLoopingPS()
    {
        foreach (ParticleSystem ps in particleSystems)
            ps.Play();
    }

    public void StopLoopingPS()
    {
        foreach (ParticleSystem ps in particleSystems)
            ps.Stop();
    }
}

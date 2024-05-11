using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class PooledPS : PooledObject
{
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private float duration;

    public void SetupPS(Vector3 spawnPos)
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            transform.position = spawnPos;
            ps.Play();
        }

        Deactivate(duration);
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

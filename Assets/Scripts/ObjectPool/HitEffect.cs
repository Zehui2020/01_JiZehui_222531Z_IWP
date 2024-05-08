using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class HitEffect : PooledObject
{
    public void SetupHitEffect(Vector3 spawnPos, Vector3 direction)
    {
        transform.position = spawnPos;
        transform.forward = direction;

        Deactivate(3f);
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
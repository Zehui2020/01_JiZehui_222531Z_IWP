using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class BulletTracer : PooledObject
{
    private TrailRenderer trailRenderer;
    private Rigidbody tracerRB;
    [SerializeField] private float tracerLifetime;

    public override void Init()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        tracerRB = GetComponent<Rigidbody>();
    }

    public void SetupTracer(Transform startPoint, Vector3 direction, float shootForce, float tracerSize)
    {
        transform.position = startPoint.position;

        trailRenderer.Clear();
        trailRenderer.startWidth = tracerSize;
        trailRenderer.endWidth = tracerSize;

        gameObject.SetActive(true);
        tracerRB.AddForce(direction.normalized * shootForce, ForceMode.Impulse);

        StartCoroutine(DeactivateRoutine());
    }

    private IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(tracerLifetime);
        Deactivate();
    }

    public void Deactivate()
    {
        Release();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("BulletTracer"))
            Deactivate();
    }
}
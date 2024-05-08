using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class BulletTracer : PooledObject
{
    private TrailRenderer trailRenderer;
    [SerializeField] private float tracerLifetime;

    public override void Init()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void SetupTracer(Vector3 startPoint, Vector3 endPoint, RaycastHit hit, float tracerSize)
    {
        StartCoroutine(DeactivateRoutine());
        StartCoroutine(DoSetupTracer(startPoint, endPoint, hit, tracerSize));
    }

    private IEnumerator DoSetupTracer(Vector3 startPoint, Vector3 endPoint, RaycastHit hit, float tracerSize)
    {
        transform.position = startPoint;
        trailRenderer.Clear();

        yield return null;

        trailRenderer.emitting = true;
        trailRenderer.startWidth = tracerSize;
        trailRenderer.endWidth = tracerSize;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDist = distance;

        while (remainingDist > 0)
        {
            transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDist / distance)));
            remainingDist -= Time.deltaTime * 200f;
            yield return null;
        }

        transform.position = endPoint;
        Deactivate();
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
        trailRenderer.emitting = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("BulletTracer"))
            Deactivate();
    }
}
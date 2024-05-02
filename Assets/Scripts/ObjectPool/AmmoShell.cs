using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class AmmoShell : PooledObject
{
    private Rigidbody shellRB;

    public override void Init()
    {
        shellRB = GetComponent<Rigidbody>();
    }

    public void SetupShell(Vector3 spawnPos, float ejectForce, float ejectUpwardForce, Vector3 torque, float lifetime)
    {
        transform.position = spawnPos;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);

        shellRB.AddForce(transform.up * ejectForce + transform.right * ejectUpwardForce, ForceMode.Impulse);
        shellRB.velocity = PlayerController.Instance.GetVelocity();
        shellRB.AddTorque(torque, ForceMode.Impulse);

        Deactivate(lifetime);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class SpitterProjectile : PooledObject
{
    private Rigidbody projectileRB;
    [SerializeField] private int damage;

    public override void Init()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    public void SetupProjectile(Vector3 spawnPos, Vector3 shootDir, float ejectForce, int damage)
    {
        transform.position = spawnPos;
        transform.forward = shootDir;
        this.damage = damage;

        gameObject.SetActive(true);

        projectileRB.velocity = PlayerController.Instance.GetVelocity();
        projectileRB.AddForce(shootDir * ejectForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.TryGetComponent<PlayerController>(out PlayerController playerController))
            return;

        playerController.TakeDamage(damage);
    }
}
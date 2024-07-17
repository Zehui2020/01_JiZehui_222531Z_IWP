using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class SpitterProjectile : PooledObject
{
    private Rigidbody projectileRB;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage;
    [SerializeField] private float lifetime;

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
        StartCoroutine(DestroyRoutine());
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<PlayerController>(out PlayerController playerController))
            playerController.TakeDamage(damage);

        if (Utility.Instance.CheckLayer(col.gameObject, enemyLayer))
            Release();
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        Release();
    }
}
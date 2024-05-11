using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class GrenadeProjectile : PooledObject
{
    private Rigidbody projectileRB;
    [SerializeField] private float explosionRadius;
    [SerializeField] private int damage;

    public override void Init()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    public void SetupProjectile(Vector3 spawnPos, Vector3 shootDir, float ejectForce, int damage)
    {
        transform.position = spawnPos;
        this.damage = damage;

        gameObject.SetActive(true);

        projectileRB.AddForce(shootDir * ejectForce, ForceMode.Impulse);
        projectileRB.velocity = PlayerController.Instance.GetVelocity();
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            PlayerStats playerStats = GetTopmostParent(col.transform).GetComponent<PlayerStats>();
            EnemyStats enemyStats = GetTopmostParent(col.transform).GetComponent<EnemyStats>();

            if (playerStats != null)
                playerStats.TakeDamage((int)(damage / 2f));
            else if (enemyStats != null)
                enemyStats.TakeDamage(damage, Vector3.zero, DamagePopup.ColorType.WHITE);
        }

        PooledPS pooledPS = ObjectPool.Instance.GetPooledObject("SmallExplosion", true).GetComponent<PooledPS>();
        pooledPS.SetupPS(transform.position);

        Release();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private Transform GetTopmostParent(Transform child)
    {
        Transform parent = child.parent;

        while (parent != null)
        {
            child = parent;
            parent = child.parent;
        }

        return child;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
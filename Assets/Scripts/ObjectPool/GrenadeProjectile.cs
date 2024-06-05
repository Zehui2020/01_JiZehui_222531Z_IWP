using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class GrenadeProjectile : PooledObject
{
    private Rigidbody projectileRB;
    [SerializeField] private ItemStats itemStats;
    [SerializeField] private float explosionRadius;
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

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            PlayerStats playerStats = col.GetComponent<PlayerStats>();
            EnemyStats enemyStats = Utility.Instance.GetTopmostParent(col.transform).GetComponent<EnemyStats>();

            float distance = Vector3.Distance(PlayerController.Instance.transform.position, col.transform.position);
            if (distance <= itemStats.minDistance)
                damage = (int)(damage * itemStats.distanceDamageModifier);

            if (playerStats != null)
                playerStats.TakeDamage(20);
            else if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage, Vector3.zero, DamagePopup.ColorType.WHITE, false);
                if (enemyStats.health <= 0)
                    PlayerController.Instance.AddPoints(100);
                else
                    PlayerController.Instance.AddPoints(50);
            }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
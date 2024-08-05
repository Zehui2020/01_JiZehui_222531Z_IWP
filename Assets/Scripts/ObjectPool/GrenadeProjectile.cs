using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;
using Unity.Burst.CompilerServices;

public class GrenadeProjectile : PooledObject
{
    private Rigidbody projectileRB;
    [SerializeField] private ItemStats itemStats;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] private int damage;

    private Weapon weapon;

    public override void Init()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    public void SetupProjectile(Weapon weapon, Vector3 spawnPos, Vector3 shootDir, float ejectForce, int damage)
    {
        transform.position = spawnPos;
        transform.forward = shootDir;
        this.damage = damage;
        this.weapon = weapon;

        gameObject.SetActive(true);

        projectileRB.velocity = PlayerController.Instance.GetVelocity();
        projectileRB.AddForce(shootDir * ejectForce, ForceMode.Impulse);
    }

    public void Explode()
    {
        Sound s = AudioManager.Instance.FindSound(Sound.SoundName.GrenadeExplode);
        ObjectPool.Instance.GetPooledObject("AudioPlayer", true).GetComponent<AudioPlayer>().SetupAudioPlayer(s);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            PlayerStats playerStats = col.GetComponent<PlayerStats>();
            Enemy enemy = Utility.Instance.GetTopmostParent(col.transform).GetComponent<Enemy>();

            if (playerStats != null)
                playerStats.TakeDamage(20);

            if (enemy != null)
            {
                if (enemy.health <= 0)
                    continue;

                bool isKnuckleActive = false;
                weapon.AddDamageModifiers(enemy, enemy.transform.position, ref isKnuckleActive);

                enemy.TakeDamage((int)(damage * weapon.totalDamageModifer), Vector3.zero, Vector3.zero, DamagePopup.ColorType.WHITE, false);

                // Chance to inflict burn
                int randNum = Random.Range(0, 100);
                if (randNum < itemStats.incendiaryChance)
                    enemy.BurnEnemy(5f, 1f, (int)((damage * itemStats.incendiaryDamageModifier) / 5f));

                if (enemy.health <= 0)
                {
                    PlayerController.Instance.AddPoints(60);
                    enemy.GetComponent<RagdollController>().ExplosionRagdoll(explosionForce, transform.position, explosionRadius);
                }
                else
                    PlayerController.Instance.AddPoints(15);
            }
        }

        PooledPS pooledPS = ObjectPool.Instance.GetPooledObject("SmallExplosion", true).GetComponent<PooledPS>();
        pooledPS.SetupPS(transform.position);

        PlayerController.Instance.powerShot = 0;
        PlayerController.Instance.RemoveStatusEffect(StatusEffect.StatusEffectType.PowerShot);

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class EnemyStats : Stats
{
    [SerializeField] protected ItemStats itemStats;
    [SerializeField] private Transform damageSpawnPoint;

    public override void TakeDamage(int damage, Vector3 position, DamagePopup.ColorType color)
    {
        // Check for crit
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.critRate)
        {
            damage = (int)(damage * (itemStats.critDamage / 100f));
            color = DamagePopup.ColorType.RED;
        }

        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true).GetComponent<DamagePopup>();
        if (position != Vector3.zero)
            damagePopup.SetupPopup(damage, position, color);
        else
            damagePopup.SetupPopup(damage, damageSpawnPoint.position, color);

        base.TakeDamage(damage);
    }
}
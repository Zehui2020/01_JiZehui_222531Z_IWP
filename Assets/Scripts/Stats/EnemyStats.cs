using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField] protected ItemStats itemStats;

    public override int TakeDamage(int damage, out bool crit)
    {
        crit = false;
        // Check for crit
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.critRate)
        {
            crit = true;
            damage = (int)(damage * (itemStats.critDamage / 100f));
        }

        return base.TakeDamage(damage, out bool critical);
    }
}

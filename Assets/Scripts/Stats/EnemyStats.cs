using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;
using System.Drawing;
using UnityEngine.UIElements;

public class EnemyStats : Stats
{
    [SerializeField] private Transform damageSpawnPoint;
    private Coroutine damageRoutine;

    public void TakeDamage(int damage, Vector3 position, Vector3 direction, DamagePopup.ColorType color, bool ignoreThreshold)
    {
        if (damageRoutine == null && !ignoreThreshold)
            damageRoutine = StartCoroutine(TakeDamageRoutine(damage, position, direction, color));
        else if (ignoreThreshold)
            damageRoutine = StartCoroutine(TakeDamageRoutine(damage, position, direction, color));
    }

    private IEnumerator TakeDamageRoutine(int damage, Vector3 position, Vector3 direction, DamagePopup.ColorType color)
    {
        if (health <= 0)
        {
            damageRoutine = null;
            yield break;
        }

        OnTakeDamage(damage, position, direction, color);

        yield return null;

        damageRoutine = null;
    }

    public virtual void OnTakeDamage(int damage, Vector3 position, Vector3 direction, DamagePopup.ColorType color)
    {
        // Check for crit
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.critRate)
        {
            damage = (int)(damage * (itemStats.critDamage / 100f));
            color = DamagePopup.ColorType.RED;
            PlayerController.Instance.Heal(itemStats.critHealAmount);
        }

        // Check for knuckle duster
        if (health >= maxHealth * itemStats.knuckleHealthThreshold)
            damage = (int)(damage * itemStats.knuckleDamageModifier);

        // Check for power shots
        int powerShots = PlayerController.Instance.powerShot;
        if (powerShots > 0)
        {
            damage = (int)(damage * (1 + itemStats.bootsDamageModifier * powerShots));
            PlayerController.Instance.powerShot = 0;
            PlayerController.Instance.RemoveStatusEffect(StatusEffect.StatusEffectType.PowerShot);
        }

        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true).GetComponent<DamagePopup>();
        if (position != Vector3.zero)
            damagePopup.SetupPopup(damage, position, color);
        else
            damagePopup.SetupPopup(damage, damageSpawnPoint.position, color);

        base.TakeDamage(damage);
    }
}
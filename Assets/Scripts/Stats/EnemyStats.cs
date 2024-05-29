using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class EnemyStats : Stats
{
    [SerializeField] private Transform damageSpawnPoint;
    private Coroutine damageRoutine;

    public event System.Action TakeDamageEvent;

    public override void TakeDamage(int damage, Vector3 position, DamagePopup.ColorType color, bool ignoreThreshold)
    {
        if (damageRoutine == null && !ignoreThreshold)
            damageRoutine = StartCoroutine(TakeDamageRoutine(damage, position, color));
        else if (ignoreThreshold)
            damageRoutine = StartCoroutine(TakeDamageRoutine(damage, position, color));
    }

    private IEnumerator TakeDamageRoutine(int damage, Vector3 position, DamagePopup.ColorType color)
    {
        if (health <= 0)
        {
            damageRoutine = null;
            yield break;
        }

        // Check for crit
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.critRate)
        {
            damage = (int)(damage * (itemStats.critDamage / 100f));
            color = DamagePopup.ColorType.RED;
            PlayerController.Instance.Heal(itemStats.critHealAmount);
        }

        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true).GetComponent<DamagePopup>();
        if (position != Vector3.zero)
            damagePopup.SetupPopup(damage, position, color);
        else
            damagePopup.SetupPopup(damage, damageSpawnPoint.position, color);

        base.TakeDamage(damage);
        TakeDamageEvent?.Invoke();

        yield return null;

        damageRoutine = null;
    }
}
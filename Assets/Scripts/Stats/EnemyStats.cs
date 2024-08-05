using System.Collections;
using UnityEngine;
using DesignPatterns.ObjectPool;

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

        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true).GetComponent<DamagePopup>();
        if (position != Vector3.zero)
            damagePopup.SetupPopup(damage, position, color);
        else
            damagePopup.SetupPopup(damage, damageSpawnPoint.position, color);

        base.TakeDamage(damage);
    }
}
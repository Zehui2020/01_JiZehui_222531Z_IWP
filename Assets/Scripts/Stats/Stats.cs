using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] protected ItemStats itemStats;
    public int health;
    public int maxHealth;

    public virtual void TakeDamage(int damage)
    {
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.blockChance)
            return;

        health -= damage;
    }

    public virtual void TakeDamage(int damage, Vector3 position, DamagePopup.ColorType color, bool ignoreTreshold) { }

    public virtual void Heal(int amount)
    {
        health += amount * itemStats.healingBonus;
        if (health > maxHealth)
            health = maxHealth;
    }
}
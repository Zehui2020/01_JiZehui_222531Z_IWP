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
        health -= damage;
    }

    public virtual void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
    }
}
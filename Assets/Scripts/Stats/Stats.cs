using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
    }

    public virtual void TakeDamage(int damage, Vector3 position, DamagePopup.ColorType color) { }

    public virtual void Heal(int amount)
    {
        health += amount;
    }
}
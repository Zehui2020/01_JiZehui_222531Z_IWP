using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int health;

    public virtual int TakeDamage(int damage, out bool crit)
    {
        crit = false;
        health -= damage;
        return damage;
    }

    public virtual void Heal(int amount)
    {
        health += amount;
    }
}
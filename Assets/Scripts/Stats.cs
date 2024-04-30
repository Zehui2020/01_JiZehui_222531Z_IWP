using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int health;
    public int armour;

    public virtual void DealDamage(int damage)
    {
        if (damage - armour <= 0)
            health -= (int)(damage * 0.1f);
        else
            health -= damage;
    }

    public virtual void Heal(int amount)
    {
        health += amount;
    }
}
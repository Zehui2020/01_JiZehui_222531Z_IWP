using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int health;

    public virtual void DealDamage(int damage)
    {
        health -= damage;
    }

    public virtual void Heal(int amount)
    {
        health += amount;
    }
}
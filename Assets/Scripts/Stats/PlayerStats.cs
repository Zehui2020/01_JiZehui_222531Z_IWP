using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public int knifeDamage;
    public int points;
    public int passiveRegenAmount;
    public float passiveRegenInterval;

    public override void TakeDamage(int damage)
    {
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.blockChance)
        {
            AudioManager.Instance.Play(Sound.SoundName.Block);
            return;
        }

        base.TakeDamage(damage);
    }

    public override void Heal(int amount)
    {
        if (health >= maxHealth)
            return;

        amount *= itemStats.healingBonus;

        base.Heal(amount);
    }
}
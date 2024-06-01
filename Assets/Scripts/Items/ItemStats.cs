using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats")]
public class ItemStats : ScriptableObject
{
    public int critRate;
    public int critDamage;
    public float sprintSpeedModifier;
    public float burnDamageModifier;

    public float distanceDamageModifier;
    public int minDistance;

    public int critHealAmount;
    public int blockChance;

    public int dynamiteExplodeDamage;
    public float dynamiteExplodeRadius;
    public float dynamiteBurnDamageModifier;

    public float fireRateModifier;
    public float relaodRateModifier;

    public float stunGrenadeRadius;
    public float stunGrenadeDuration;

    public void ResetStats()
    {
        critRate = 1;
        critDamage = 200;
        sprintSpeedModifier = 1;
        burnDamageModifier = 1;

        distanceDamageModifier = 1;
        minDistance = 3;

        critHealAmount = 0;
        blockChance = 0;

        dynamiteExplodeDamage = 0;
        dynamiteExplodeRadius = 0;
        dynamiteBurnDamageModifier = 0;

        fireRateModifier = 1;
        relaodRateModifier = 1;

        stunGrenadeRadius = 0;
        stunGrenadeDuration = 2;
    }
}
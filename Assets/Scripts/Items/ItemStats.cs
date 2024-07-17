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
    public float minDistance;

    public int critHealAmount;
    public int blockChance;

    public float dynamiteExplodeDamage;
    public float dynamiteExplodeRadius;
    public float dynamiteBurnDamageModifier;

    public float fireRateModifier;
    public float relaodRateModifier;

    public float stunGrenadeRadius;
    public float stunGrenadeDuration;
    public float stunGrenadeCooldown;

    public float shungiteHealing;

    public int drumReloadPercentage;

    public int healingBonus;

    public float jitbStunRadius;
    public float jitbStunDuration;

    public float knuckleHealthThreshold;
    public float knuckleDamageModifier;

    public float bootsDamageModifier;
    public float bootsStackLimit;
    public float bootsSprintDuration;

    public float magSizeModifier;

    public int doublePointsChance;

    public int incendiaryChance;
    public float incendiaryDamageModifier;

    public float piercingChance;

    public void ResetStats()
    {
        critRate = 1;
        critDamage = 200;
        sprintSpeedModifier = 1;
        burnDamageModifier = 1;

        distanceDamageModifier = 0;
        minDistance = 2;

        critHealAmount = 0;
        blockChance = 0;

        dynamiteExplodeDamage = 0;
        dynamiteExplodeRadius = 0;
        dynamiteBurnDamageModifier = 0;

        fireRateModifier = 1;
        relaodRateModifier = 1;

        stunGrenadeRadius = 0;
        stunGrenadeDuration = 2;
        stunGrenadeCooldown = 5;

        shungiteHealing = 0;

        drumReloadPercentage = 0;

        healingBonus = 1;

        jitbStunRadius = 0;
        jitbStunDuration = 5;

        knuckleHealthThreshold = 0.9f;
        knuckleDamageModifier = 0;

        bootsDamageModifier = 0;
        bootsStackLimit = 5;
        bootsSprintDuration = 2;

        magSizeModifier = 1;

        doublePointsChance = 0;

        incendiaryChance = 0;
        incendiaryDamageModifier = 0;

        piercingChance = 0;
    }
}
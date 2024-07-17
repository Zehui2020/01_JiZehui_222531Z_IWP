using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/IncendiartyRounds")]
public class IncendiaryRounds : Item
{
    [SerializeField] private int incendiaryChance;
    [SerializeField] private float baseIncendiaryDamage;
    [SerializeField] private float stackIncendiaryDamage;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.incendiaryChance += incendiaryChance;
        itemStats.incendiaryDamageModifier += baseIncendiaryDamage;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.incendiaryChance += incendiaryChance;
        itemStats.incendiaryDamageModifier += stackIncendiaryDamage;
    }
}
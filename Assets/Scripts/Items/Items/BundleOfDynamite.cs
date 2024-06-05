using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/BundleOfDynamite")]
public class BundleOfDynamite : Item
{
    [SerializeField] private int baseExplodeDamage;
    [SerializeField] private float baseBurnDamage;
    [SerializeField] private float baseExplodeRadius;

    [SerializeField] private int stackExplodeDamage;
    [SerializeField] private float stackBurnDamage;
    [SerializeField] private float stackExplodeRadius;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.dynamiteExplodeDamage += baseExplodeDamage;
        itemStats.dynamiteBurnDamageModifier += baseBurnDamage;
        itemStats.dynamiteExplodeRadius += baseExplodeRadius;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.dynamiteExplodeDamage += stackExplodeDamage;
        itemStats.dynamiteBurnDamageModifier += stackBurnDamage;
        itemStats.dynamiteExplodeRadius += stackExplodeRadius;
    }
}
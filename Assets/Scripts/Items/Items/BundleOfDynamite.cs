using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/BundleOfDynamite")]
public class BundleOfDynamite : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.dynamiteExplodeDamage += 40;
        itemStats.dynamiteBurnDamageModifier += 0.2f;
        itemStats.dynamiteExplodeRadius += 3;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.dynamiteExplodeDamage += 25;
        itemStats.dynamiteBurnDamageModifier += 0.2f;
        itemStats.dynamiteExplodeRadius += 2;
    }
}
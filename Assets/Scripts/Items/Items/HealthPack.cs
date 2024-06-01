using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HealthPack")]
public class HealthPack : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.healingBonus *= 2;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.healingBonus *= 2;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HoloDie")]
public class HoloDie : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.blockChance += 15;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.blockChance += 10;
    }
}
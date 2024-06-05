using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HoloDie")]
public class HoloDie : Item
{
    [SerializeField] private int baseBlockChance;
    [SerializeField] private int stackBlockChance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.blockChance += baseBlockChance;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.blockChance += stackBlockChance;
    }
}
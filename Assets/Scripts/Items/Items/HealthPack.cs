using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HealthPack")]
public class HealthPack : Item
{
    [SerializeField] private int healingBonus;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.healingBonus *= healingBonus;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.healingBonus *= healingBonus;
    }
}
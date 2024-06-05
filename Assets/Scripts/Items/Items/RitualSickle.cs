using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/RitualSickle")]
public class RitualSickle : Item
{
    [SerializeField] private int baseCritRate;
    [SerializeField] private int baseCritHeal;

    [SerializeField] private int stackCritHeal;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.critRate += baseCritRate;
        itemStats.critHealAmount += baseCritHeal;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.critHealAmount += stackCritHeal;
    }
}
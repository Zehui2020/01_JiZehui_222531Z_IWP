using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/RitualSickle")]
public class RitualSickle : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.critRate += 5;
        itemStats.critHealAmount += 4;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.critHealAmount += 2;
    }
}
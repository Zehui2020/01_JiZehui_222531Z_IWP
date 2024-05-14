using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gasoline")]
public class Gasoline : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.burnDamageModifier *= 2;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.burnDamageModifier *= 1.5f;
    }
}
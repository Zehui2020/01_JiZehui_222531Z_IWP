using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/StunGrenade")]
public class StunGrenade : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.stunGrenadeRadius += 10;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.stunGrenadeRadius += 5;
    }
}
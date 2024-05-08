using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HDHUD")]
public class HDHUD : Item
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.critRate += 10;
        itemStats.critDamage += 5;
    }
}
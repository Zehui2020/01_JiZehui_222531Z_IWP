using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gasoline")]
public class Gasoline : Item
{
    [SerializeField] private float baseBurnDamage;
    [SerializeField] private float stackBurnDamage;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.burnDamageModifier *= baseBurnDamage;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.burnDamageModifier *= stackBurnDamage;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/AmmoStash")]
public class AmmoStash : Item
{
    [SerializeField] private int magSizeModifierIncrease;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.magSizeModifier += magSizeModifierIncrease;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.magSizeModifier += magSizeModifierIncrease;
    }
}
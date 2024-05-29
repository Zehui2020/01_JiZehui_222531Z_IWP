using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ColdOne")]
public class ColdOne : Item
{
    public static event System.Action IncreaseFireRate;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.fireRateModifier += 0.15f;
        IncreaseFireRate?.Invoke();
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.fireRateModifier += 0.15f;
        IncreaseFireRate?.Invoke();
    }
}
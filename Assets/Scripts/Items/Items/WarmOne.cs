using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/WarmOne")]
public class WarmOne : Item
{
    public static event System.Action IncreaseReloadRate;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.relaodRateModifier += 0.15f;
        IncreaseReloadRate?.Invoke();
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.relaodRateModifier += 0.15f;
        IncreaseReloadRate?.Invoke();
    }
}
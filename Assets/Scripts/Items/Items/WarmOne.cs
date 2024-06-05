using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/WarmOne")]
public class WarmOne : Item
{
    public static event System.Action IncreaseReloadRate;
    [SerializeField] private float baseReloadMod;
    [SerializeField] private float stackReloadMod;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.relaodRateModifier += baseReloadMod;
        IncreaseReloadRate?.Invoke();
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.relaodRateModifier += stackReloadMod;
        IncreaseReloadRate?.Invoke();
    }
}
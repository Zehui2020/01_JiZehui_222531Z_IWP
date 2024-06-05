using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ColdOne")]
public class ColdOne : Item
{
    public static event System.Action IncreaseFireRate;

    [SerializeField] private float baseFireRate;
    [SerializeField] private float stackFireRate;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.fireRateModifier += baseFireRate;
        IncreaseFireRate?.Invoke();
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.fireRateModifier += stackFireRate;
        IncreaseFireRate?.Invoke();
    }
}
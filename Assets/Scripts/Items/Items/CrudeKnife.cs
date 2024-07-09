using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/CrudeKnife")]
public class CrudeKnife : Item
{
    [SerializeField] private float baseDistDamageMod;

    [SerializeField] private float stackDistDamageMod;
    [SerializeField] private float stackDistanceMod;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.distanceDamageModifier *= baseDistDamageMod;
        PlayerController.Instance.ShowCrudeKnifeRadius();
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.distanceDamageModifier *= stackDistDamageMod;
        itemStats.minDistance += stackDistanceMod;
        PlayerController.Instance.ShowCrudeKnifeRadius();
    }
}
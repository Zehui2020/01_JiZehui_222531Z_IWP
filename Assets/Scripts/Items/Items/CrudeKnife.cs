using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/CrudeKnife")]
public class CrudeKnife : Item
{
    [SerializeField] private float baseDistMod;

    [SerializeField] private float stackDistMod;
    [SerializeField] private int stackMinDistance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.distanceDamageModifier *= baseDistMod;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.distanceDamageModifier *= stackDistMod;
        itemStats.minDistance += stackMinDistance;
    }
}
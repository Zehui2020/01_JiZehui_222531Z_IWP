using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/CorruptedBoots")]
public class CorruptedBoots : Item
{
    [SerializeField] private float stackDamageModifier;
    [SerializeField] private float stackSprintDuration;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.bootsDamageModifier += stackDamageModifier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.bootsDamageModifier += stackDamageModifier;
        itemStats.bootsSprintDuration *= stackSprintDuration;
    }
}
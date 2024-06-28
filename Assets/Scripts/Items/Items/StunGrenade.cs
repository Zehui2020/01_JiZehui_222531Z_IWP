using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/StunGrenade")]
public class StunGrenade : Item
{
    [SerializeField] private int baseStunRadius;
    [SerializeField] private int stackStunRadius;
    [SerializeField] private float stunCooldownModifier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.stunGrenadeRadius += baseStunRadius;
        itemStats.stunGrenadeCooldown *= stunCooldownModifier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.stunGrenadeRadius += stackStunRadius;
        itemStats.stunGrenadeCooldown *= stunCooldownModifier;
    }
}
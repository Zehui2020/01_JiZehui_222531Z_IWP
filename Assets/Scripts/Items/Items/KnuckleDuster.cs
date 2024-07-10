using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/KnuckleDuster")]
public class KnuckleDuster : Item
{
    [SerializeField] private float baseDamageModifier;
    [SerializeField] private float stackDamageModifier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.knuckleDamageModifier += baseDamageModifier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.knuckleDamageModifier += stackDamageModifier;
    }
}
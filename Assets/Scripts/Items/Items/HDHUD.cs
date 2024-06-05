using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/HDHUD")]
public class HDHUD : Item
{
    [SerializeField] private int critRate;
    [SerializeField] private int critDmg;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.critRate += critRate;
        itemStats.critDamage += critDmg;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.critRate += critRate;
        itemStats.critDamage += critDmg;
    }
}
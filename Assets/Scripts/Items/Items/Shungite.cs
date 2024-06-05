using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Shungite")]
public class Shungite : Item
{
    [SerializeField] private float baseHeal;
    [SerializeField] private float stackHeal;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.shungiteHealing += baseHeal;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.shungiteHealing += stackHeal;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Abascus")]
public class Abascus : Item
{
    [SerializeField] private int occuranceChance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.doublePointsChance += occuranceChance;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.doublePointsChance += occuranceChance;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/PiercingRounds")]
public class PiercingRounds : Item
{
    [SerializeField] private int piercingChance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.piercingChance += piercingChance;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.piercingChance += piercingChance;
    }
}
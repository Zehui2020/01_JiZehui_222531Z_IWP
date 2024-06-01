using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Shungite")]
public class Shungite : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.shungiteHealing += 0.03f;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.shungiteHealing += 0.03f;
    }
}
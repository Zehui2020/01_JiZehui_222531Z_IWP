using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/AdrenalineShot")]
public class AdrenalineShot : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.sprintSpeedModifier += 0.15f;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.sprintSpeedModifier += 0.1f;
    }
}
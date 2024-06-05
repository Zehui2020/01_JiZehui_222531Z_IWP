using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/AdrenalineShot")]
public class AdrenalineShot : Item
{
    [SerializeField] private float baseModifier;
    [SerializeField] private float stackModifier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.sprintSpeedModifier += baseModifier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.sprintSpeedModifier += stackModifier;
    }
}
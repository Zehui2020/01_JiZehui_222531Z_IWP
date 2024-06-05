using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/X-KILL Drum")]
public class XKILLDrum : Item
{
    [SerializeField] private int baseChance;
    [SerializeField] private int sackChance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.drumReloadPercentage += baseChance;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.drumReloadPercentage += sackChance;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/X-KILL Drum")]
public class XKILLDrum : Item
{
    public override void Initialize()
    {
        base.Initialize();
        itemStats.drumReloadPercentage += 20;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.drumReloadPercentage += 10;
    }
}
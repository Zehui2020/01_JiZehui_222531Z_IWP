using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats")]
public class ItemStats : ScriptableObject
{
    public int critRate;
    public int critDamage;
    public float sprintSpeedModifier;
    public float burnDamageModifier;

    public void ResetStats()
    {
        critRate = 1;
        critDamage = 200;
        sprintSpeedModifier = 1;
        burnDamageModifier = 1;
    }
}
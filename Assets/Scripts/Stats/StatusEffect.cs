using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public enum StatusEffectType { Burn, Stun, MoveSpeed }
    public StatusEffectType statusEffectType;

    public Sprite icon;
}
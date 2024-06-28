using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public enum StatusEffectType { Burn, Stun, MoveSpeed, Floodlight, GasTank, ReinforcedSteel, Tires, StunGrenadeCD}
    public StatusEffectType statusEffectType;

    public enum StatusEffectCategory { Buff, Debuff, VehiclePart }
    public StatusEffectCategory statusEffectCategory;

    public Sprite icon;
}
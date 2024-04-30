using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { Melee, Ranged }
    public WeaponType weaponType;

    public int damagePerBullet;
    public float attackInterval;
    public float recoil;
    public float reloadDuration;
    public int ammoPerMag;

    public float unADSSway;
    public float ADSSway;
}
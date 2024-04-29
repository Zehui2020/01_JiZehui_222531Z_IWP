using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { Melee, Ranged }
    public WeaponType weaponType;

    public float damagePerBullet;
    public float fireRate;
    public float recoil;
    public float reloadDuration;
}
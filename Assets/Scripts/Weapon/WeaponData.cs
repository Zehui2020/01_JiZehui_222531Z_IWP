using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum Weapon { Pistol, Shotgun, Rifle }
    public Weapon weapon;

    public enum WeaponType { Melee, Ranged }
    public WeaponType weaponType;

    public int damagePerBullet;
    public float headshotMultiplier;
    public int ammoPerMag;

    public float ADSRecoil;
    public float unADSRecoil;

    public float ADSBulletSpreadAccuracy;
    public float unADSBulletSpreadAccuracy;

    public float unADSSway;
    public float ADSSway;

    public float ADSCamShake;
    public float unADSCamShake;

    public float ADSCamShakeDuration;
    public float unADSCamShakeDuration;

    public float ADSZoomAmount;
    public float ADSZoomDuration;

    public float shellEjectForce;
    public float shellEjectUpwardForce;
    public float shellEjectTorque;
}
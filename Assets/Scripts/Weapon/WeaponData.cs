using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum Weapon { Pistol, Shotgun, Rifle, LMG, AK47, Flamethrower, SawnOff, GrenadeLauncher }
    public Weapon weapon;

    public enum WeaponType { Melee, Ranged }
    public WeaponType weaponType;

    public enum FireType { SemiAuto, FullAuto }
    public FireType fireType;

    public int damagePerBullet;
    public float headshotMultiplier;
    public int ammoPerMag;

    public float ADSRecoilX;
    public float ADSRecoilY;
    public float unADSRecoilX;
    public float unADSRecoilY;

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
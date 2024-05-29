using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private int bulletsPerShot;

    public override void UseWeapon()
    {
        base.UseWeapon();
        muzzleFlash.PlayPS();
        DoRaycast(0.07f, bulletsPerShot);
        ApplyRecoil();
    }

    public override void ReloadWeapon()
    {
        ammoCount++;
        totalAmmo--;

        if (ammoCount >= weaponData.ammoPerMag || totalAmmo <= 0)
            weaponAnimator.SetTrigger("finishReloading");

        base.ReloadWeapon();
    }
}
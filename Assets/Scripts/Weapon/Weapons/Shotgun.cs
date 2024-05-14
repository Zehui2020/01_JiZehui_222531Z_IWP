using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private float bulletsPerShot;

    public override void UseWeapon()
    {
        base.UseWeapon();
        muzzleFlash.PlayPS();

        for (int i = 0; i < bulletsPerShot; i++)
        {
            DoRaycast(0.1f);
        }

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private float bulletsPerShot;

    public override void UseWeapon()
    {
        base.UseWeapon();

        for (int i = 0; i < bulletsPerShot; i++)
        {
            DoRaycast(0.1f);
        }
    }

    public override void ReloadWeapon()
    {
        ammoCount++;
        totalAmmo--;

        if (ammoCount >= weaponData.ammoPerMag)
            weaponAnimator.SetTrigger("finishReloading");
    }
}
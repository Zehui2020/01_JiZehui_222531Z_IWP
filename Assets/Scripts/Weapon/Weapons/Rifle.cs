using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    public override void UseWeapon()
    {
        base.UseWeapon();

        EjectShell("PistolShell");
        muzzleFlash.PlayPS();

        DoRaycast(0.07f);
        ApplyRecoil();
    }

    public override void ReloadWeapon()
    {
        totalAmmo -= weaponData.ammoPerMag - ammoCount;
        ammoCount = weaponData.ammoPerMag;
    }
}
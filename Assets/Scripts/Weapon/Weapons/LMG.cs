using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMG : Weapon
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
        if (totalAmmo <= weaponData.ammoPerMag)
        {
            ammoCount = totalAmmo;
            totalAmmo = 0;
        }
        else
        {
            totalAmmo -= weaponData.ammoPerMag - ammoCount;
            ammoCount = weaponData.ammoPerMag;
        }

        base.ReloadWeapon();
    }
}
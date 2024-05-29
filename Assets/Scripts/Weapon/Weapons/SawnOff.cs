using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawnOff : Weapon
{
    [SerializeField] private int bulletsPerShot;

    public override void UseWeapon()
    {
        base.UseWeapon();
        muzzleFlash.PlayPS();

        DoRaycast(0.07f, bulletsPerShot);

        ApplyRecoil();
    }

    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);
        switch (newState)
        {
            case WeaponState.RELOAD:
                weaponAnimator.SetInteger("ammo", ammoCount);
                break;
        }
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
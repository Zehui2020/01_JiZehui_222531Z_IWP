using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawnOff : Weapon
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
        totalAmmo -= weaponData.ammoPerMag - ammoCount;
        ammoCount = weaponData.ammoPerMag;
    }
}
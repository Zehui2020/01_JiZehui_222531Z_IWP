using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private int bulletsPerShot;

    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);

        switch (newState)
        {
            case WeaponState.USE:
                AudioManager.Instance.PlayOneShot(Sound.SoundName.ShotgunShoot);
                break;
            case WeaponState.HIDE:
                AudioManager.Instance.Stop(Sound.SoundName.ShotgunReload);
                break;
        }
    }

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

        if (ammoCount >= Mathf.CeilToInt(weaponData.ammoPerMag * itemStats.magSizeModifier) || totalAmmo <= 0)
            weaponAnimator.SetTrigger("finishReloading");

        base.ReloadWeapon();
    }
}
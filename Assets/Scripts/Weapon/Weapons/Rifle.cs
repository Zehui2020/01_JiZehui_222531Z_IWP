using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);

        switch (newState)
        {
            case WeaponState.USE:
                AudioManager.Instance.PlayOneShot(Sound.SoundName.RifleShoot);
                break;
            case WeaponState.RELOAD:
                Sound s = AudioManager.Instance.FindSound(Sound.SoundName.RifleReload);
                AudioManager.Instance.SetPitch(Sound.SoundName.RifleReload, s.pitch * itemStats.relaodRateModifier);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.RifleReload);
                break;
            case WeaponState.HIDE:
                AudioManager.Instance.Stop(Sound.SoundName.RifleReload);
                break;
        }
    }

    public override void UseWeapon()
    {
        base.UseWeapon();

        EjectShell("PistolShell");
        muzzleFlash.PlayPS();
        DoRaycast(0.07f, 1);
        ApplyRecoil();
    }

    public override void ReloadWeapon()
    {
        if (totalAmmo <= Mathf.CeilToInt(weaponData.ammoPerMag * itemStats.magSizeModifier))
        {
            ammoCount = totalAmmo;
            totalAmmo = 0;
        }
        else
        {
            totalAmmo -= Mathf.CeilToInt(weaponData.ammoPerMag * itemStats.magSizeModifier) - ammoCount;
            ammoCount = Mathf.CeilToInt(weaponData.ammoPerMag * itemStats.magSizeModifier);
        }

        base.ReloadWeapon();
    }
}
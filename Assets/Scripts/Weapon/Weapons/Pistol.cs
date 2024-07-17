using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);

        switch (newState)
        {
            case WeaponState.USE:
                AudioManager.Instance.PlayOneShot(Sound.SoundName.PistolShoot);
                break;
            case WeaponState.RELOAD:
                Sound s = AudioManager.Instance.FindSound(Sound.SoundName.PistolReload);
                AudioManager.Instance.SetPitch(Sound.SoundName.PistolReload, s.pitch * itemStats.relaodRateModifier);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.PistolReload);
                break;
            case WeaponState.HIDE:
                AudioManager.Instance.Stop(Sound.SoundName.PistolReload);
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
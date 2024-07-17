using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47 : Weapon
{
    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);
        switch (newState)
        {
            case WeaponState.USE:
                AudioManager.Instance.PlayOneShot(Sound.SoundName.AKShoot);
                break;
            case WeaponState.HIDE:
                AudioManager.Instance.Stop(Sound.SoundName.AKReload);
                break;
            case WeaponState.RELOAD:
                Sound s = AudioManager.Instance.FindSound(Sound.SoundName.AKReload);
                AudioManager.Instance.SetPitch(Sound.SoundName.AKReload, s.pitch * itemStats.relaodRateModifier);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.AKReload);
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
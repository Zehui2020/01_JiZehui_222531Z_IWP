using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMG : Weapon
{
    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);

        switch (newState)
        {
            case WeaponState.RELOAD:
                Sound s = AudioManager.Instance.FindSound(Sound.SoundName.LMGReload);
                AudioManager.Instance.SetPitch(Sound.SoundName.LMGReload, s.pitch * itemStats.relaodRateModifier);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.LMGReload);
                break;
            case WeaponState.HIDE:
                AudioManager.Instance.Stop(Sound.SoundName.LMGReload);
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
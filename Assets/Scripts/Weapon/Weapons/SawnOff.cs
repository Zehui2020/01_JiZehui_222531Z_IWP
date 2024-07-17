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
            case WeaponState.USE:
                AudioManager.Instance.PlayOneShot(Sound.SoundName.SawnOffShoot);
                break;
            case WeaponState.HIDE:
                AudioManager.Instance.Stop(Sound.SoundName.SawnOffReload);
                break;
            case WeaponState.RELOAD:
                Sound s = AudioManager.Instance.FindSound(Sound.SoundName.SawnOffReload);
                AudioManager.Instance.SetPitch(Sound.SoundName.SawnOffReload, s.pitch * itemStats.relaodRateModifier);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.SawnOffReload);
                weaponAnimator.SetInteger("ammo", ammoCount);
                break;
        }
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
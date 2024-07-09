using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class GrenadeLauncher : Weapon
{
    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);

        switch (newState)
        {
            case WeaponState.USE:
                AudioManager.Instance.PlayOneShot(Sound.SoundName.GLauncherShoot);
                break;
        }
    }

    public override void UseWeapon()
    {
        base.UseWeapon();
        muzzleFlash.PlayPS();

        Vector3 shootDir = GetShotDirection(Camera.main.transform.forward);
        GrenadeProjectile grenadeProjectile = ObjectPool.Instance.GetPooledObject("GrenadeProjectile", false).GetComponent<GrenadeProjectile>();
        grenadeProjectile.SetupProjectile(firePoint.position, shootDir, weaponData.shellEjectForce, (int)(weaponData.damagePerBullet * upgradeDamageModifier));

        ApplyRecoil();
    }

    public override void ReloadWeapon()
    {
        ammoCount++;
        totalAmmo--;

        AudioManager.Instance.PlayOneShot(Sound.SoundName.GLauncherReloadOnce);
        if (ammoCount >= weaponData.ammoPerMag || totalAmmo <= 0)
            weaponAnimator.SetTrigger("finishReloading");

        base.ReloadWeapon();
    }
}
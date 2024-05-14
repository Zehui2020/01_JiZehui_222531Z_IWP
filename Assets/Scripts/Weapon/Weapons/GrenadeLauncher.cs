using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class GrenadeLauncher : Weapon
{
    public override void UseWeapon()
    {
        base.UseWeapon();
        muzzleFlash.PlayPS();

        Vector3 shootDir = GetShotDirection(Camera.main.transform.forward);
        GrenadeProjectile grenadeProjectile = ObjectPool.Instance.GetPooledObject("GrenadeProjectile", false).GetComponent<GrenadeProjectile>();
        grenadeProjectile.SetupProjectile(firePoint.position, shootDir, weaponData.shellEjectForce, weaponData.damagePerBullet);

        ApplyRecoil();
    }

    public override void ReloadWeapon()
    {
        ammoCount++;
        totalAmmo--;

        if (ammoCount >= weaponData.ammoPerMag || totalAmmo <= 0)
            weaponAnimator.SetTrigger("finishReloading");

        base.ReloadWeapon();
    }
}
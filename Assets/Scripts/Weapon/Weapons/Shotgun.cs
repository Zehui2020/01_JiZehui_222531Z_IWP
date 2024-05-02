using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public override void UseWeapon()
    {
        ammoCount--;

        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, targetLayer))
            return;

        Stats stat = hit.collider.GetComponent<Stats>();
        if (stat == null)
            return;

        stat.DealDamage(weaponData.damagePerBullet);
    }

    public override void ReloadWeapon()
    {
        ammoCount++;
        totalAmmo--;

        if (ammoCount >= weaponData.ammoPerMag)
        {
            //ammoCount = weaponData.ammoPerMag;
            weaponAnimator.SetTrigger("finishReloading");
        }
    }
}
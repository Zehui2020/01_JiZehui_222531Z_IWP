using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public override void UseWeapon()
    {
        ammoCount--;
        EjectShell("PistolShell");

        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, targetLayer))
            return;

        Stats stat = hit.collider.GetComponent<Stats>();
        if (stat == null)
            return;

        stat.DealDamage(weaponData.damagePerBullet);
    }

    public override void ReloadWeapon()
    {
        totalAmmo -= weaponData.ammoPerMag - ammoCount;
        ammoCount = weaponData.ammoPerMag;
    }
}
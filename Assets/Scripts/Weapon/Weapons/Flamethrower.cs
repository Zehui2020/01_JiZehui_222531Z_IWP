using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
    [SerializeField] private FlamethrowerRadius flamethrowerRadius;

    [SerializeField] private float baseBurnDuration;
    [SerializeField] private float baseBurnInterval;

    public override void InitWeapon(Action swapEvent)
    {
        base.InitWeapon(swapEvent);
    }

    public override void UseWeapon()
    {
        base.UseWeapon();
        muzzleFlash.PlayLoopingPS();

        foreach (Enemy enemy in flamethrowerRadius.enemiesInRadius)
        {
            enemy.TakeDamage(weaponData.damagePerBullet, Vector3.zero, DamagePopup.ColorType.WHITE);
            enemy.BurnEnemy(baseBurnDuration, baseBurnInterval, (int)(weaponData.damagePerBullet / 2f));
        }
    }

    public override void UpdateWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        base.UpdateWeapon(horizontal, vertical, mouseX, mouseY, isGrounded);
        flamethrowerRadius.CheckEnemyAlive();

        if (ammoCount <= 0 || Input.GetMouseButtonUp(0))
            weaponAnimator.SetTrigger("stopUse");
    }

    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);
        if (newState != WeaponState.USE)
            muzzleFlash.StopLoopingPS();
    }

    public override void ReloadWeapon()
    {
        totalAmmo -= weaponData.ammoPerMag - ammoCount;
        ammoCount = weaponData.ammoPerMag;
    }
}
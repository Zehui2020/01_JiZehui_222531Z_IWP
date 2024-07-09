using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
    [SerializeField] private FlamethrowerRadius flamethrowerRadius;

    [SerializeField] private float baseBurnDuration;
    [SerializeField] private float baseBurnInterval;

    public override void ChangeState(WeaponState newState)
    {
        base.ChangeState(newState);

        switch (newState)
        {
            case WeaponState.USE:
                AudioManager.Instance.StopFadeRoutine(Sound.SoundName.FlamethrowerShoot);
                AudioManager.Instance.Play(Sound.SoundName.FlamethrowerShoot);
                break;
            case WeaponState.RELOAD:
                Sound s = AudioManager.Instance.FindSound(Sound.SoundName.FlamethrowerReload);
                AudioManager.Instance.SetPitch(Sound.SoundName.FlamethrowerReload, s.pitch * itemStats.relaodRateModifier);
                AudioManager.Instance.PlayOneShot(Sound.SoundName.FlamethrowerReload);
                break;
            case WeaponState.HIDE:
                AudioManager.Instance.Stop(Sound.SoundName.FlamethrowerReload);
                break;
        }

        if (newState != WeaponState.USE)
        {
            muzzleFlash.StopLoopingPS();
            AudioManager.Instance.FadeSound(false, Sound.SoundName.FlamethrowerShoot, 1, 0);
        }
    }

    public override void UseWeapon()
    {
        base.UseWeapon();
        muzzleFlash.PlayLoopingPS();

        List<Enemy> enemiesCopy = new List<Enemy>(flamethrowerRadius.enemiesInRadius);

        foreach (Enemy enemy in enemiesCopy)
        {
            if (enemy == null)
                continue;

            if (!flamethrowerRadius.enemiesInRadius.Contains(enemy))
                continue;

            int damage = (int)(weaponData.damagePerBullet * upgradeDamageModifier);
            float distance = Vector3.Distance(PlayerController.Instance.transform.position, enemy.transform.position);
            if (distance <= itemStats.minDistance)
                damage = (int)(weaponData.damagePerBullet * itemStats.distanceDamageModifier * upgradeDamageModifier);

            enemy.TakeDamage(damage, Vector3.zero, Vector3.zero, DamagePopup.ColorType.WHITE, true);
            if (enemy.health <= 0)
                PlayerController.Instance.AddPoints(20);

            enemy.BurnEnemy(baseBurnDuration, baseBurnInterval, (int)(weaponData.damagePerBullet / 2f));
        }
    }

    public override void UpdateWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        base.UpdateWeapon(horizontal, vertical, mouseX, mouseY, isGrounded);

        if (ammoCount <= 0 || Input.GetMouseButtonUp(0))
        {
            weaponAnimator.SetTrigger("stopUse");
        }
    }

    public override void ReloadWeapon()
    {
        if (totalAmmo <= weaponData.ammoPerMag)
        {
            ammoCount = totalAmmo;
            totalAmmo = 0;
        }
        else
        {
            totalAmmo -= weaponData.ammoPerMag - ammoCount;
            ammoCount = weaponData.ammoPerMag;
        }

        base.ReloadWeapon();
    }
}
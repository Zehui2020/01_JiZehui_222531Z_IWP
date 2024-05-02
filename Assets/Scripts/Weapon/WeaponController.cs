using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private List<Weapon> weaponPool;
    private int currentWeapon;

    public event System.Action SwapWeaponEvent;

    public void InitWeaponController()
    {
        SwapWeaponEvent += OnSwitchWeapon;

        foreach (Weapon weapon in weaponPool)
        {
            weapon.InitWeapon(SwapWeaponEvent);

            if (weapon != weapons[currentWeapon])
                weapon.gameObject.SetActive(false);
        }
    }

    public bool UseWeapon()
    {
        if (weapons.Count == 0)
            return false;

        return weapons[currentWeapon].OnUse();
    }

    public void ADSWeapon()
    {
        if (weapons.Count == 0)
            return;

        weapons[currentWeapon].ToggleADS();
    }

    public void ReloadWeapon()
    {
        if (weapons.Count == 0)
            return;

        weapons[currentWeapon].OnReload();
    }

    public void SwitchWeapon()
    {
        if (weapons.Count > 1)
            weapons[currentWeapon].OnSwap();
    }

    public void OnSwitchWeapon()
    {
        currentWeapon++;
        if (currentWeapon > weapons.Count - 1)
            currentWeapon = 0;

        if (!weapons[currentWeapon].gameObject.activeInHierarchy)
            weapons[currentWeapon].gameObject.SetActive(true);
        else
            weapons[currentWeapon].OnShow();
    }

    public void ReplaceWeapon(WeaponData.Weapon newWeapon)
    {
        Weapon targetWeapon = null;
        foreach (Weapon weapon in weaponPool)
        {
            if (weapon.GetWeapon() != newWeapon)
                continue;

            targetWeapon = weapon;
        }

        if (weapons.Count < 2)
        {
            weapons.Add(targetWeapon);
            weapons[currentWeapon].OnSwap();
        }
        else
        {
            weapons[currentWeapon].OnReturnToPool();
            weapons[currentWeapon] = targetWeapon;
            targetWeapon.gameObject.SetActive(true);
        }
    }

    public void UpdateCurrentWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        weapons[currentWeapon].UpdateWeapon(horizontal, vertical, mouseX, mouseY, isGrounded);
    }

    public float GetWeaponCamShakeAmount()
    {
        return weapons[currentWeapon].GetCamShakeAmount();
    }

    public float GetWeaponCamShakeDuration()
    {
        return weapons[currentWeapon].GetCamShakeDuration();
    }

    public float GetWeaponZoomAmount()
    {
        return weapons[currentWeapon].GetWeaponZoomAmount();
    }

    public float GetWeaponZoomDuration()
    {
        return weapons[currentWeapon].GetWeaponZoomDuration();
    }
}
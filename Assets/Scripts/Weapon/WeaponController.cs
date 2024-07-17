using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private List<Weapon> weaponPool;
    private UIController uiController;
    private int currentWeapon;

    public void InitWeaponController()
    {
        uiController = GetComponent<UIController>();

        foreach (Weapon weapon in weapons)
        {
            weapon.InitWeapon();
            weapon.SwapWeaponEvent += OnSwitchWeapon;
            weapon.UseWeaponEvent += uiController.UpdateAmmoCount;
            weapon.ReloadWeaponEvent += uiController.UpdateAmmoCount;
            weapon.RestockWeaponEvent += uiController.UpdateAmmoCount;
        }

        foreach (Weapon weapon in weaponPool)
        {
            weapon.InitWeapon();
            weapon.gameObject.SetActive(false);
        }

        uiController.SetWeaponCount(weapons.Count);

        if (weapons.Count < 2)
            uiController.SetWeaponUIs(weapons[currentWeapon], null);
        else
            uiController.SetWeaponUIs(weapons[currentWeapon], weapons[(currentWeapon + 1) % (weapons.Count - 1)]);

        uiController.UpdateCrosshair(GetWeaponCrosshair());
    }

    public Weapon GetRandomWeaponFromPool()
    {
        int randNum = Random.Range(0, weaponPool.Count);
        return weaponPool[randNum];
    }

    public bool UseWeapon()
    {
        if (weapons.Count == 0)
            return false;

        return weapons[currentWeapon].OnUse();
    }

    public void ADSWeapon(bool isADS)
    {
        if (weapons.Count == 0)
            return;

        weapons[currentWeapon].SetADS(isADS);
    }

    public bool ReloadWeapon()
    {
        if (weapons.Count == 0)
            return false;

        return weapons[currentWeapon].OnReload();
    }

    public void SwitchWeapon()
    {
        if (weapons.Count > 1)
            weapons[currentWeapon].OnSwap();
    }

    public void OnSwitchWeapon(Weapon weapon)
    {
        Weapon prevWeapon = weapons[currentWeapon];

        currentWeapon++;
        if (currentWeapon > weapons.Count - 1)
            currentWeapon = 0;

        Weapon newWeapon = weapons[currentWeapon];

        if (!newWeapon.gameObject.activeInHierarchy)
            newWeapon.gameObject.SetActive(true);
        else
            newWeapon.OnShow();

        uiController.SetWeaponUIs(newWeapon, prevWeapon);
        uiController.UpdateCrosshair(GetWeaponCrosshair());
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

        if (targetWeapon == null)
            return;

        CompanionManager.Instance.ShowWeaponPickupMessage(targetWeapon.weaponData.weapon);

        if (weapons.Count < 2)
        {
            weapons.Add(targetWeapon);
            weaponPool.Remove(targetWeapon);
            weapons[currentWeapon].OnSwap();
        }
        else
        {
            weapons[currentWeapon].SwapWeaponEvent -= OnSwitchWeapon;
            weapons[currentWeapon].UseWeaponEvent -= uiController.UpdateAmmoCount;
            weapons[currentWeapon].ReloadWeaponEvent -= uiController.UpdateAmmoCount;
            weapons[currentWeapon].RestockWeaponEvent -= uiController.UpdateAmmoCount;

            weapons[currentWeapon].OnReturnToPool();
            weaponPool.Add(weapons[currentWeapon]);

            weapons[currentWeapon] = targetWeapon;
            weaponPool.Remove(targetWeapon);

            targetWeapon.gameObject.SetActive(true);
            uiController.OnReplaceWeapon(targetWeapon);
        }

        targetWeapon.SwapWeaponEvent += OnSwitchWeapon;
        targetWeapon.UseWeaponEvent += uiController.UpdateAmmoCount;
        targetWeapon.ReloadWeaponEvent += uiController.UpdateAmmoCount;
        targetWeapon.RestockWeaponEvent += uiController.UpdateAmmoCount;

        uiController.SetWeaponCount(weapons.Count);
        uiController.UpdateCrosshair(GetWeaponCrosshair());
    }

    public void HideCurrentWeapon()
    {
        weapons[currentWeapon].HideWeaponWithoutSwap();
    }

    public void ShowCurrentWeapon()
    {
        weapons[currentWeapon].OnShow();
    }

    public void UpdateCurrentWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        weapons[currentWeapon].UpdateWeapon(horizontal, vertical, mouseX, mouseY, isGrounded);
    }

    public WeaponData.FireType GetFireType()
    {
        return weapons[currentWeapon].GetFireType();
    }

    public float GetWeaponCamShakeAmount()
    {
        return weapons[currentWeapon].GetCamShakeAmount();
    }

    public float GetWeaponCamShakeFrequency()
    {
        return weapons[currentWeapon].GetCamShakeFrequency();
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

    public bool RestockWeapon()
    {
        return weapons[currentWeapon].RestockWeapon();
    }

    public void RefillAmmoClip()
    {
        weapons[currentWeapon].RefillAmmoClip();
    }

    public void UpgradeWeapon()
    {
        weapons[currentWeapon].UpgradeWeapon();
    }

    public Sprite GetWeaponCrosshair()
    {
        return weapons[currentWeapon].weaponData.weaponCrosshair;
    }

    public void SetSprinting(bool isSprinting)
    {
        weapons[currentWeapon].ToggleSprint(isSprinting);
    }

    public Weapon GetCurrentWeapon()
    {
        return weapons[currentWeapon];
    }
}
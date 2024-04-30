using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons;
    private int currentWeapon;

    public void InitWeaponController()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.InitWeapon();
        }
    }

    public void UseWeapon()
    {
        if (weapons.Count == 0)
            return;

        weapons[currentWeapon].OnUse();
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

        weapons[currentWeapon].Reload();
    }

    public void SwitchWeapon()
    {
        currentWeapon++;
        if (currentWeapon > weapons.Count)
            currentWeapon = 0;
    }

    public void ReplaceWeapon(Weapon newWeapon)
    {
        weapons[currentWeapon] = newWeapon;
    }

    public void UpdateCurrentWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        weapons[currentWeapon].UpdateWeapon(horizontal, vertical, mouseX, mouseY, isGrounded);
    }
}
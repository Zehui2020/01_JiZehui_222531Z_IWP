using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponAmmo;
    [SerializeField] private TextMeshProUGUI weaponLevel;

    public void SetWeaponUI(Weapon weapon)
    {
        UpdateAmmoCount(weapon.ammoCount, weapon.totalAmmo);
        weaponIcon.sprite = weapon.weaponData.weaponIcon;
        weaponLevel.text = "Level " + weapon.level;
    }

    public void UpdateAmmoCount(int currentAmmo, int maxAmmo)
    {
        weaponAmmo.text = currentAmmo + "/" + maxAmmo;
    }
}
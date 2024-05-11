using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class WeaponPickup : PooledObject
{
    [SerializeField] private WeaponData.Weapon weapon;

    public override void Init()
    {
        objectName = weapon.ToString();
    }

    public void PickupWeapon()
    {
        PlayerController.Instance.ReplaceWeapon(weapon);

        Release();
        gameObject.SetActive(false);
    }
}
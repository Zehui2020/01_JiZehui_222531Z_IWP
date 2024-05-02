using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupTest : MonoBehaviour
{
    [SerializeField] private WeaponData.Weapon weapon;

    private void OnCollisionEnter(Collision collision)
    {
        WeaponController weaponController = collision.collider.GetComponent<WeaponController>();
        if (weaponController != null)
            weaponController.ReplaceWeapon(weapon);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        WeaponController weaponController = collision.collider.GetComponent<WeaponController>();
        if (weaponController != null)
            weaponController.ReplaceWeapon(WeaponData.Weapon.Shotgun);
    }
}

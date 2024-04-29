using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] protected int ammoCount;
    [SerializeField] protected int totalAmmo;

    private Coroutine useWeaponRoutine;
    private Coroutine reloadWeaponRoutine;

    public void OnUse()
    {
        if (useWeaponRoutine != null)
            return;

        if (weaponData.weaponType == WeaponData.WeaponType.Ranged && 
            useWeaponRoutine == null && 
            reloadWeaponRoutine == null &&
            ammoCount > 0)
            useWeaponRoutine = StartCoroutine(UseWeapon());

        else if (weaponData.weaponType == WeaponData.WeaponType.Melee)
            useWeaponRoutine = StartCoroutine(UseWeapon());
    }

    public void Reload()
    {
        if (reloadWeaponRoutine == null && ammoCount < weaponData.ammoPerMag)
            reloadWeaponRoutine = StartCoroutine(DoReload());
    }

    public virtual void UseWeaponLogic()
    {
        if (Physics.Raycast(firePoint.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, targetLayer))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
        }

        ammoCount--;
    }

    private IEnumerator UseWeapon()
    {
        UseWeaponLogic();

        yield return new WaitForSeconds(weaponData.attackInterval);

        useWeaponRoutine = null;
    }

    private IEnumerator DoReload()
    {
        yield return new WaitForSeconds(weaponData.reloadDuration);

        totalAmmo -= weaponData.ammoPerMag - ammoCount;
        ammoCount = weaponData.ammoPerMag;

        reloadWeaponRoutine = null;
    }
}
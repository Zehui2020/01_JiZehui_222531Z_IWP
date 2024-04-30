using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private WeaponSway weaponSway;
    private Animator weaponAnimator;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private Vector3 unADSPos;
    [SerializeField] private Vector3 ADSPos;

    [SerializeField] protected int ammoCount;
    [SerializeField] protected int totalAmmo;

    private Coroutine useWeaponRoutine;
    private Coroutine reloadWeaponRoutine;
    private bool isADS = false;

    public void InitWeapon()
    {
        weaponSway = GetComponent<WeaponSway>();
        weaponAnimator = GetComponent<Animator>();
    }

    public void UpdateWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        if (weaponSway == null)
            return;

        if (isADS)
            transform.localPosition = Vector3.Lerp(transform.localPosition, ADSPos, Time.deltaTime * 20f);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, unADSPos, Time.deltaTime * 20f);

        weaponSway.UpdateWeaponSway(horizontal, vertical, mouseX, mouseY, isGrounded);
    }

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

    public void ToggleADS()
    {
        isADS = !isADS;

        if (isADS)
            weaponSway.smooth = weaponData.ADSSway;
        else
            weaponSway.smooth = weaponData.unADSSway;
    }

    public void Reload()
    {
        if (reloadWeaponRoutine == null && ammoCount < weaponData.ammoPerMag)
            reloadWeaponRoutine = StartCoroutine(DoReload());
    }

    public virtual void UseWeaponLogic()
    {
        weaponAnimator.SetTrigger("use");
        ammoCount--;

        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, targetLayer))
            return;

        Stats stat = hit.collider.GetComponent<Stats>();
        if (stat == null)
            return;

        stat.DealDamage(weaponData.damagePerBullet);
    }

    private IEnumerator UseWeapon()
    {
        UseWeaponLogic();

        yield return new WaitForSeconds(weaponData.attackInterval);

        useWeaponRoutine = null;
    }

    private IEnumerator DoReload()
    {
        weaponAnimator.SetTrigger("reload");

        yield return new WaitForSeconds(weaponData.reloadDuration);

        totalAmmo -= weaponData.ammoPerMag - ammoCount;
        ammoCount = weaponData.ammoPerMag;

        reloadWeaponRoutine = null;
    }
}
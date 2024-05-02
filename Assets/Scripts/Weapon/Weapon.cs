using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class Weapon : MonoBehaviour
{
    public enum WeaponState
    {
        HIDE,
        SHOW,
        READY,
        USE,
        RELOAD
    }
    public WeaponState currentState;

    private WeaponSway weaponSway;
    protected Animator weaponAnimator;
    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] private int hiddenLayer;
    [SerializeField] private int weaponLayer;

    [SerializeField] private Vector3 unADSPos;
    [SerializeField] private Quaternion unADSRotation;
    [SerializeField] private Vector3 ADSPos;
    [SerializeField] private Quaternion ADSRotation;

    [SerializeField] protected int ammoCount;
    [SerializeField] protected int totalAmmo;

    [SerializeField] private Transform shellSpawnPoint;
    [SerializeField] private float shellLifetime;

    private bool isADS = false;
    private bool invokeSwapEvent = true;
    private bool returnToPool = false;
    private System.Action SwapWeaponEvent;

    public void InitWeapon(System.Action swapEvent)
    {
        SwapWeaponEvent = swapEvent;
        weaponSway = GetComponent<WeaponSway>();
        weaponAnimator = GetComponent<Animator>();
    }

    public void ChangeState(WeaponState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case WeaponState.HIDE:
                weaponAnimator.SetTrigger("hide");
                break;
            case WeaponState.SHOW:
                SetGameLayerRecursive(gameObject, weaponLayer);
                weaponAnimator.SetTrigger("show");
                break;
            case WeaponState.READY:
                break;
            case WeaponState.USE:
                weaponAnimator.SetTrigger("use");
                break;
            case WeaponState.RELOAD:
                weaponAnimator.SetTrigger("reload");
                break;
            default:
                break;
        }
    }

    public void UpdateWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        if (weaponSway == null)
            return;

        if (isADS)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, ADSPos, Time.deltaTime * 20f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, ADSRotation, Time.deltaTime * 20f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, unADSPos, Time.deltaTime * 20f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, unADSRotation, Time.deltaTime * 20f);
        }

        weaponSway.UpdateWeaponSway(horizontal, vertical, mouseX, mouseY, isGrounded);
    }

    public void ToggleADS()
    {
        isADS = !isADS;

        if (isADS)
            weaponSway.smooth = weaponData.ADSSway;
        else
            weaponSway.smooth = weaponData.unADSSway;
    }

    public bool OnUse()
    {
        if (currentState == WeaponState.USE || 
            currentState == WeaponState.HIDE ||
            currentState == WeaponState.SHOW ||
            currentState == WeaponState.RELOAD)
            return false;

        if (weaponData.weaponType == WeaponData.WeaponType.Ranged &&
            ammoCount > 0)
        {
            ChangeState(WeaponState.USE);
            return true;
        }
        else if (weaponData.weaponType == WeaponData.WeaponType.Melee)
        {
            ChangeState(WeaponState.USE);
            return true;
        }

        return false;
    }

    public void OnReload()
    {
        if (currentState != WeaponState.RELOAD && 
            ammoCount < weaponData.ammoPerMag)
            ChangeState(WeaponState.RELOAD);
    }

    public void OnSwap()
    {
        ChangeState(WeaponState.HIDE);
    }

    public void OnShow()
    {
        ChangeState(WeaponState.SHOW);
    }

    public void OnReturnToPool()
    {
        returnToPool = true;
        ChangeState(WeaponState.HIDE);
    }

    public virtual void ReloadWeapon() { }

    public virtual void UseWeapon() { }

    public void Swap()
    {
        if (returnToPool)
            return;

        if (invokeSwapEvent)
            SwapWeaponEvent.Invoke();

        SetGameLayerRecursive(gameObject, hiddenLayer);
        invokeSwapEvent = true;
    }

    public void HideWeaponWithoutSwap()
    {
        invokeSwapEvent = false;
        ChangeState(WeaponState.HIDE);
    }

    public void CheckReturnToPool()
    {
        if (returnToPool)
        {
            returnToPool = false;
            gameObject.SetActive(false);
        }
    }

    public void EjectShell(string shellName)
    {
        AmmoShell shell = ObjectPool.Instance.GetPooledObject(shellName, false).GetComponent<AmmoShell>();
        shell.SetupShell(shellSpawnPoint.position, weaponData.shellEjectForce, weaponData.shellEjectUpwardForce, new Vector3(GetRandomTorque(), GetRandomTorque(), 0), shellLifetime);
    }

    public float GetCamShakeAmount()
    {
        if (isADS)
            return weaponData.ADSCamShake;
        else
            return weaponData.unADSCamShake;
    }

    public float GetCamShakeDuration()
    {
        if (isADS)
            return weaponData.ADSCamShakeDuration;
        else
            return weaponData.unADSCamShakeDuration;
    }

    protected Vector3 GetShotDirection(Vector3 direction)
    {
        Vector3 BSAOffset = new Vector3(GetRandomOffsetBSA(), GetRandomOffsetBSA(), 0);
        return direction + BSAOffset.normalized;
    }

    private float GetRandomOffsetBSA()
    {
        if (!isADS)
            return Random.Range(-weaponData.unADSBulletSpreadAccuracy, weaponData.unADSBulletSpreadAccuracy);
        else
            return Random.Range(-weaponData.ADSBulletSpreadAccuracy, weaponData.ADSBulletSpreadAccuracy);
    }

    public float GetWeaponZoomAmount()
    {
        return weaponData.ADSZoomAmount;
    }

    public float GetWeaponZoomDuration()
    {
        return weaponData.ADSZoomDuration;
    }

    public WeaponData.Weapon GetWeapon()
    {
        return weaponData.weapon;
    }

    protected float GetRandomTorque()
    {
        return Random.Range(-weaponData.shellEjectTorque, weaponData.shellEjectTorque);
    }

    private void SetGameLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, layer);
        }
    }
}
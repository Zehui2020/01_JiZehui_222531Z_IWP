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
    public WeaponData weaponData;
    [SerializeField] protected Transform firePoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] private LayerMask hiddenLayer;
    [SerializeField] private LayerMask weaponLayer;

    [SerializeField] private Vector3 unADSPos;
    [SerializeField] private Quaternion unADSRotation;
    [SerializeField] private Vector3 ADSPos;
    [SerializeField] private Quaternion ADSRotation;

    [SerializeField] protected int ammoCount;
    [SerializeField] protected int totalAmmo;

    [SerializeField] private Transform shellSpawnPoint;
    [SerializeField] private float shellLifetime;

    [SerializeField] protected ParticleSystemEmitter muzzleFlash;

    private bool isADS = false;
    private bool invokeSwapEvent = true;
    private bool returnToPool = false;
    private System.Action SwapWeaponEvent;

    public virtual void InitWeapon(System.Action swapEvent)
    {
        SwapWeaponEvent = swapEvent;
        weaponSway = GetComponent<WeaponSway>();
        weaponAnimator = GetComponent<Animator>();
    }

    public virtual void ChangeState(WeaponState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case WeaponState.HIDE:
                weaponAnimator.SetTrigger("hide");
                break;
            case WeaponState.SHOW:
                SetGameLayerRecursive(gameObject, LayerMaskToLayerNumber(weaponLayer));
                weaponAnimator.SetTrigger("show");
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

    public virtual void UpdateWeapon(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
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

    public virtual void UseWeapon() { ammoCount--; }

    public virtual void DoRaycast(float tracerSize)
    {
        Vector3 shootDir = GetShotDirection(Camera.main.transform.forward);
        Ray ray = new Ray(Camera.main.transform.position, shootDir);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ~playerLayer))
        {
            ShootTracer(firePoint.position + (shootDir * 500f), new RaycastHit(), tracerSize);
            return;
        }
            
        ShootTracer(hit.point, hit, tracerSize);

        EnemyStats enemyStats = GetTopmostParent(hit.collider.transform).GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            ObjectPool.Instance.GetPooledObject("StoneHitEffect", true).GetComponent<HitEffect>().SetupHitEffect(hit.point, -hit.normal);
            return;
        }

        int damage = weaponData.damagePerBullet;
        DamagePopup.ColorType colorType;

        if (hit.collider.CompareTag("Head"))
        {
            colorType = DamagePopup.ColorType.YELLOW;
            damage = (int)(damage * weaponData.headshotMultiplier);
        }
        else
            colorType = DamagePopup.ColorType.WHITE;

        ObjectPool.Instance.GetPooledObject("BloodHitEffect", true).GetComponent<HitEffect>().SetupHitEffect(hit.point, -Camera.main.transform.forward);

        enemyStats.TakeDamage(damage, hit.point, colorType);
    }

    public void Swap()
    {
        if (returnToPool)
            return;

        if (invokeSwapEvent)
            SwapWeaponEvent.Invoke();

        SetGameLayerRecursive(gameObject, LayerMaskToLayerNumber(hiddenLayer));
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

    protected void ShootTracer(Vector3 endPos, RaycastHit hit, float tracerSize)
    {
        BulletTracer tracer = ObjectPool.Instance.GetPooledObject("BulletTracer", true).GetComponent<BulletTracer>();
        tracer.SetupTracer(firePoint.position, endPos, hit, tracerSize);
    }

    protected void ApplyRecoil()
    {
        PlayerController.Instance.ApplyRecoil(GetRecoil().x, GetRecoil().y);
    }

    public Vector2 GetRecoil()
    {
        if (isADS)
            return new Vector2(weaponData.ADSRecoilX, weaponData.ADSRecoilY);
        else
            return new Vector2(weaponData.unADSRecoilX, weaponData.unADSRecoilY);
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
        return (direction + BSAOffset).normalized;
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

    public WeaponData.FireType GetFireType()
    {
        return weaponData.fireType;
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

    private int LayerMaskToLayerNumber(LayerMask layerMask)
    {
        int maskValue = layerMask.value;
        int layerNumber = 0;
        while (maskValue > 0)
        {
            maskValue >>= 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }

    private Transform GetTopmostParent(Transform child)
    {
        Transform parent = child.parent;

        while (parent != null)
        {
            child = parent;
            parent = child.parent;
        }

        return child;
    }
}
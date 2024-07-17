using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;
using Unity.Burst.CompilerServices;

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
    [SerializeField] private Sound.SoundName dryFireSound;

    public int ammoCount;
    public int totalAmmo;
    private float fireRate;
    private float reloadRate;
    private float totalDamageModifer = 1f;
    public int level;
    protected float upgradeDamageModifier = 1f;

    [SerializeField] private Transform shellSpawnPoint;
    [SerializeField] private float shellLifetime;

    [SerializeField] protected ParticleSystemEmitter muzzleFlash;
    [SerializeField] protected ItemStats itemStats;

    private bool isADS = false;
    private bool invokeSwapEvent = true;
    private bool returnToPool = false;

    public event System.Action<Weapon> UseWeaponEvent;
    public event System.Action<Weapon> ReloadWeaponEvent;
    public event System.Action<Weapon> SwapWeaponEvent;
    public event System.Action<Weapon> RestockWeaponEvent;

    public virtual void InitWeapon()
    {
        weaponSway = GetComponent<WeaponSway>();
        weaponAnimator = GetComponent<Animator>();
        ammoCount = weaponData.ammoPerMag;
        fireRate = 1f;
        reloadRate = 1f;
        level = 1;

        ColdOne.IncreaseFireRate += IncreaseFireRate;
        WarmOne.IncreaseReloadRate += IncreaseReloadRate;
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
        if (weaponSway == null || currentState == WeaponState.SHOW || currentState == WeaponState.HIDE)
            return;

        weaponSway.UpdateWeaponSway(horizontal, vertical, mouseX, mouseY, isGrounded);

        if (isADS)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, ADSPos, Time.deltaTime * 20f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, weaponSway.swayPos + weaponSway.bobPosition, Time.deltaTime * weaponSway.smooth);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(weaponSway.swayEulerRot) * Quaternion.Euler(weaponSway.bobEulerRotation), Time.deltaTime * weaponSway.smoothRot);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, ADSRotation, Time.deltaTime * 20f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, unADSPos, Time.deltaTime * 20f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, weaponSway.swayPos + weaponSway.bobPosition, Time.deltaTime * weaponSway.smooth);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, unADSRotation, Time.deltaTime * 20f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(weaponSway.swayEulerRot) * Quaternion.Euler(weaponSway.bobEulerRotation), Time.deltaTime * weaponSway.smoothRot);
        }

        weaponAnimator.SetFloat("fireRate", fireRate);
        weaponAnimator.SetFloat("reloadRate", reloadRate);
    }

    public void SetADS(bool ADS)
    {
        isADS = ADS;

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
        else if (weaponData.weaponType == WeaponData.WeaponType.Ranged &&
            ammoCount <= 0)
        {
            AudioManager.Instance.OnlyPlayAfterSoundEnds(dryFireSound);
            return false;
        }
        else if (weaponData.weaponType == WeaponData.WeaponType.Melee)
        {
            ChangeState(WeaponState.USE);
            return true;
        }

        return false;
    }

    public bool OnReload()
    {
        if (currentState != WeaponState.RELOAD && 
            currentState != WeaponState.HIDE &&
            currentState != WeaponState.SHOW &&
            ammoCount < weaponData.ammoPerMag &&
            totalAmmo > 0)
        {
            ChangeState(WeaponState.RELOAD);
            return true;
        }

        return false;
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

    public virtual void ReloadWeapon() { ReloadWeaponEvent?.Invoke(this); }

    public virtual void UseWeapon() { ammoCount--; UseWeaponEvent?.Invoke(this); }
    public virtual void UpgradeWeapon() { upgradeDamageModifier += 1f; level++; }

    public virtual bool DoRaycast(float tracerSize, int numberOfRaycasts)
    {
        bool isHit = false;
        bool headshot = false;
        bool isKnuckleActive = false;
        int powerShots = PlayerController.Instance.powerShot;

        Dictionary<EnemyStats, bool> enemyHits = new Dictionary<EnemyStats, bool>();

        for (int i = 0; i < numberOfRaycasts; i++)
        {
            Vector3 shootDir = GetShotDirection(Camera.main.transform.forward);
            Ray ray = new Ray(Camera.main.transform.position, shootDir);
            RaycastHit hit;

            totalDamageModifer = upgradeDamageModifier;

            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ~playerLayer))
            {
                ShootTracer(firePoint.position + (shootDir * 500f), new RaycastHit(), tracerSize);
                continue;
            }

            ShootTracer(hit.point, hit, tracerSize);

            EnemyStats enemyStats = Utility.Instance.GetTopmostParent(hit.transform).GetComponent<EnemyStats>();

            if (enemyStats == null)
            {
                ObjectPool.Instance.GetPooledObject("StoneHitEffect", true).GetComponent<HitEffect>().SetupHitEffect(hit.point, -hit.normal);
                continue;
            }

            ObjectPool.Instance.GetPooledObject("BloodHitEffect", true).GetComponent<HitEffect>().SetupHitEffect(hit.point, -Camera.main.transform.forward);

            if (enemyStats.health <= 0)
                continue;

            int damage = weaponData.damagePerBullet;
            DamagePopup.ColorType colorType;

            // Check for headshot
            if (hit.collider.CompareTag("Head"))
            {
                colorType = DamagePopup.ColorType.YELLOW;
                damage = (int)(damage * weaponData.headshotMultiplier);
                headshot = true;
            }
            else
                colorType = DamagePopup.ColorType.WHITE;

            if (!enemyHits.ContainsKey(enemyStats))
                enemyHits.Add(enemyStats, headshot);

            Vector3 hitDir = (transform.position - hit.point).normalized;

            // Check for crude knife
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist <= itemStats.minDistance)
                totalDamageModifer += itemStats.distanceDamageModifier;

            // Check for knuckle duster
            if (enemyStats.health >= enemyStats.maxHealth * itemStats.knuckleHealthThreshold || isKnuckleActive)
            {
                isKnuckleActive = true;
                totalDamageModifer += itemStats.knuckleDamageModifier;
            }

            // Check for power shots
            if (powerShots > 0)
                totalDamageModifer += (itemStats.bootsDamageModifier * powerShots);

            // Tally up damage
            if (totalDamageModifer > 0)
                damage = (int)(damage * totalDamageModifer);
            Debug.Log(totalDamageModifer);
            enemyStats.TakeDamage(damage, hit.point, -hitDir, colorType, true);

            isHit = true;
        }

        // Reset power shots
        if (powerShots > 0 && isHit)
        {
            PlayerController.Instance.powerShot = 0;
            PlayerController.Instance.RemoveStatusEffect(StatusEffect.StatusEffectType.PowerShot);
        }

        foreach (KeyValuePair<EnemyStats, bool> enemyStat in enemyHits)
        {
            if (enemyStat.Key.health <= 0)
            {
                if (enemyStat.Value)
                    PlayerController.Instance.AddPoints(100);
                else
                    PlayerController.Instance.AddPoints(60);
            }
            else if (numberOfRaycasts > 1)
                PlayerController.Instance.AddPoints(30);
            else
                PlayerController.Instance.AddPoints(10);
        }

        return isHit;
    }

    private void ApplyDamageModifiers(Collider hitCollider, EnemyStats enemyStats)
    {
    }

    public void Swap()
    {
        if (returnToPool)
            return;

        if (invokeSwapEvent)
            SwapWeaponEvent?.Invoke(this);

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

    public float GetCamShakeFrequency()
    {
        if (isADS)
            return weaponData.ADSCamShakeFrequency;
        else
            return weaponData.unADSCamShakeFrequency;
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

    public void ToggleSprint(bool isSprinting)
    {
        if (isSprinting)
            weaponSway.SetBobExaggeration(weaponData.sprintBob);
        else
            weaponSway.SetBobExaggeration(weaponData.walkBob);
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

    public void SetCanADS(int allow)
    {
        if (allow == 0)
            PlayerController.Instance.SetCanADS(false);
        else
            PlayerController.Instance.SetCanADS(true);
    }

    private void IncreaseFireRate()
    {
        fireRate = itemStats.fireRateModifier;
    }

    private void IncreaseReloadRate()
    {
        reloadRate = itemStats.relaodRateModifier;
    }

    protected void CheckGivePoints(EnemyStats enemyHit, bool headshot)
    {
        if (enemyHit == null)
            return;

        if (enemyHit.health > 0)
            PlayerController.Instance.AddPoints(10);
        else
        {
            if (headshot)
                PlayerController.Instance.AddPoints(100);
            else
                PlayerController.Instance.AddPoints(60);
        }
    }

    public bool RestockWeapon()
    {
        if (totalAmmo == weaponData.maxAmmo)
            return false;

        totalAmmo = weaponData.maxAmmo;
        RestockWeaponEvent?.Invoke(this);
        return true;
    }

    public void RefillAmmoClip()
    {
        ammoCount = weaponData.ammoPerMag;
        RestockWeaponEvent?.Invoke(this);
    }

    public void PlayAudio(Sound.SoundName soundName)
    {
        AudioManager.Instance.PlayOneShot(soundName);
    }
}
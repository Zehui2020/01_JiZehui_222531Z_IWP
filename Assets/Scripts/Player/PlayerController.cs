using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerStats
{
    public static PlayerController Instance;
    private MovementController movementController;
    private WeaponController weaponController;
    private UIController uiController;
    private CameraController cameraController;
    private ItemManager itemManager;
    [SerializeField] private KnifeController knifeController;

    [SerializeField] private LayerMask enemyLayer;
    private Enemy enemyToLookAt;

    [SerializeField] private MovementData movementData;
    private Rigidbody playerRB;

    private bool isDisabled = false;
    private bool isADS = false;
    private bool canADS = true;
    private IInteractable collidedInteractable;

    public static event System.Action<int> OnUpdatePoints;
    private Coroutine OnMoveRoutine;
    private Coroutine stunEnemyRoutine;
    private Coroutine ShungiteHealingRoutine;
    private Coroutine burnRoutine;
    private Coroutine shockRoutine;

    [SerializeField] private CompanionMessenger companionMessenger;

    public List<VehiclePart.VehiclePartType> vehicleParts = new List<VehiclePart.VehiclePartType>();

    private void Awake()
    {
        Instance = this;

        // Get player components
        movementController = GetComponent<MovementController>();
        weaponController = GetComponent<WeaponController>();
        uiController = GetComponent<UIController>();
        cameraController = GetComponent<CameraController>();
        itemManager = GetComponent<ItemManager>();
        playerRB = GetComponent<Rigidbody>();

        // Initialize components
        movementController.IntializeMovementController();
        weaponController.InitWeaponController();
        cameraController.InitCameraController();
        knifeController.InitKnifeController();
        uiController.InitUIController();
        itemManager.InitItemManager();

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        itemStats.ResetStats();
        StartCoroutine(PassiveHealing());
    }

    private void Start()
    {
        OnUpdatePoints?.Invoke(points);
        CompanionManager.Instance.ShowMessages(companionMessenger.introMessages);
        System.Action onMessageFinishHandler = null;
        onMessageFinishHandler = () =>
        {
            EnemySpawner.Instance.StartWave(2f);
            Objective objective = new Objective(Objective.ObjectiveType.Normal, "Survive for 3 waves (0/3)");
            ObjectiveManager.Instance.AddObjective(objective);
            CompanionManager.Instance.OnMessageFinish -= onMessageFinishHandler;
        };
        CompanionManager.Instance.OnMessageFinish += onMessageFinishHandler;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (vehicleParts.Contains(VehiclePart.VehiclePartType.Gas_Tank) && damage > 3)
            BurnPlayer(5f, 1f, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ConsoleManager.Instance.SetConsole();

        if (Input.GetKeyDown(KeyCode.Return))
            ConsoleManager.Instance.OnInputCommand();

        if (Input.GetKeyDown(KeyCode.P))
            CompanionManager.Instance.SkipMessage();

        if (isDisabled || Time.timeScale == 0)
            return;
            
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        movementController.CheckGroundCollision();
        movementController.HandleMovment(horizontal, vertical);

        if (Input.GetKeyUp(KeyCode.Space))
            movementController.HandleJump();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            movementController.ToggleCrouch();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementController.ToggleSprint();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (weaponController.ReloadWeapon())
                OnReload();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SetCanADS(true);
            SetADS(false);
            weaponController.SwitchWeapon();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (knifeController.Knife())
                weaponController.HideCurrentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (collidedInteractable != null)
                collidedInteractable.OnInteract();
        }

        if (weaponController.GetFireType() == WeaponData.FireType.SemiAuto)
        {
            if (Input.GetMouseButtonDown(0))
                UseCurrentWeapon();
        }
        else if (weaponController.GetFireType() == WeaponData.FireType.FullAuto)
        {
            if (Input.GetMouseButton(0))
                UseCurrentWeapon();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetADS(true);
            cameraController.SetADS(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            SetADS(false);
            cameraController.SetADS(false);
        }

        weaponController.UpdateCurrentWeapon(horizontal, vertical, mouseX, mouseY, movementController.isGrounded);

        uiController.UpdateStaminaBar(movementController.stamina, movementData.maxStamina);
        uiController.UpdateHealthBar(health, maxHealth);

        transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        CheckEnemyToLookAt();

        if (itemStats.shungiteHealing == 0)
            return;

        if (OnMoveRoutine == null && !movementController.isMoving)
            OnMoveRoutine = StartCoroutine(OnStopMoving());

        if (movementController.isMoving)
        {
            if (OnMoveRoutine != null) StopCoroutine(OnMoveRoutine);
            if (ShungiteHealingRoutine != null) StopCoroutine(ShungiteHealingRoutine);

            OnMoveRoutine = null;
            ShungiteHealingRoutine = null;
        }
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
    }

    private void CheckEnemyToLookAt()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);

        foreach (RaycastHit hit in hits)
        {
            if (Utility.Instance.GetTopmostParent(hit.collider.transform).TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (enemyToLookAt != null)
                    enemyToLookAt.SetHealthbar(false);
                enemyToLookAt = enemy;
                enemyToLookAt.SetHealthbar(true);
                return;
            }
        }

        if (enemyToLookAt != null)
            enemyToLookAt.SetHealthbar(false);
    }

    private void SetADS(bool ADS)
    {
        if (isADS == ADS || !canADS)
            return;

        isADS = ADS;
        weaponController.ADSWeapon(isADS);
        uiController.OnADS(isADS);

        if (isADS)
        {
            cameraController.Zoom(weaponController.GetWeaponZoomAmount(), weaponController.GetWeaponZoomDuration());
            movementController.SetCanSprint(false);
        }
        else
        {
            movementController.SetCanSprint(true);
            cameraController.Zoom(60, weaponController.GetWeaponZoomDuration());
        }
    }

    private void OnReload()
    {
        SetADS(false);
        SetCanADS(false);

        if (stunEnemyRoutine == null && itemStats.stunGrenadeRadius > 0)
            stunEnemyRoutine = StartCoroutine(StunEnemyRoutine());
    }

    private IEnumerator StunEnemyRoutine()
    {
        ApplyStatusEffect(StatusEffect.StatusEffectType.StunGrenadeCD, true, StatusEffect.StatusEffectCategory.Debuff, itemStats.stunGrenadeCooldown);

        Collider[] colliders = Physics.OverlapSphere(transform.position, itemStats.stunGrenadeRadius);
        foreach (Collider col in colliders)
        {
            if (!Utility.Instance.GetTopmostParent(col.transform).TryGetComponent<Enemy>(out Enemy enemy))
                continue;

            enemy.StunEnemy(itemStats.stunGrenadeDuration);
        }

        yield return new WaitForSeconds(itemStats.stunGrenadeCooldown);

        stunEnemyRoutine = null;
    }

    public void SetCanADS(bool ads)
    {
        canADS = ads;
    }

    private void UseCurrentWeapon()
    {
        if (weaponController.UseWeapon())
            ShakeCamera(weaponController.GetWeaponCamShakeAmount(), weaponController.GetWeaponCamShakeFrequency(), weaponController.GetWeaponCamShakeDuration());
    }

    public void ApplyRecoil(float recoilX, float recoilY)
    {
        cameraController.ApplyRecoil(recoilX, recoilY);
    }

    public void SetDontUseStamina(float duration)
    {
        StartCoroutine(DontUseStamina(duration));
    }

    private IEnumerator DontUseStamina(float duration)
    {
        movementController.SetUseStamina(false);
        yield return new WaitForSeconds(duration);
        movementController.SetUseStamina(true);
    }

    public void AddItem(Item item)
    {
        itemManager.AddItem(item);
        uiController.OnPickupItem(item);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            collidedInteractable = interactable;
            interactable.OnEnterRange();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            collidedInteractable = null;
            interactable.OnExitRange();
        }
    }

    public int GetPoints()
    {
        return points;
    }

    public void AddPoints(int amount)
    {
        points += amount;
        OnUpdatePoints?.Invoke(points);
    }

    public void DeductPoints(int amount)
    {
        points -= amount;
        OnUpdatePoints?.Invoke(points);
    }

    public Vector3 GetVelocity()
    {
        return playerRB.velocity;
    }

    public void ShowCurrentWeapon()
    {
        weaponController.ShowCurrentWeapon();
    }

    public Weapon GetRandomWeapon()
    {
        return weaponController.GetRandomWeaponFromPool();
    }
    
    public void ReplaceWeapon(WeaponData.Weapon weapon)
    {
        weaponController.ReplaceWeapon(weapon);
    }

    public void UpgradeCurrentWeapon()
    {
        weaponController.UpgradeWeapon();
    }

    public bool RestockCurrentWeapon()
    {
        return weaponController.RestockWeapon();
    }

    public void RefillAmmoClip()
    {
        weaponController.RefillAmmoClip();
    }

    public void ShakeCamera(float intensity, float frequency, float duration)
    {
        cameraController.ShakeCamera(intensity, frequency, duration);
    }

    private IEnumerator OnStopMoving()
    {
        yield return new WaitForSeconds(3f);
        ShungiteHealingRoutine = StartCoroutine(DoShungiteHealing());
    }

    private IEnumerator DoShungiteHealing()
    {
        while (true)
        {
            Heal((int)(maxHealth * itemStats.shungiteHealing));
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator PassiveHealing()
    {
        while (true)
        {
            if (health < maxHealth)
                yield return null;

            Heal(passiveRegenAmount);
            yield return new WaitForSeconds(passiveRegenInterval);
        }
    }

    public void AddVehiclePart(VehiclePart vehiclePart)
    {
        vehicleParts.Add(vehiclePart.vehiclePartType);
        collidedInteractable = null;
    }

    public void SetMoveSpeedModifier(float modifier)
    {
        movementController.SetMoveSpeedModifier(modifier);
    }

    public void SetStaminaModifier(float modifier)
    {
        movementController.SetStaminaModifier(modifier);
    }

    public void BurnPlayer(float duration, float interval, int damage)
    {
        if (burnRoutine != null)
            StopCoroutine(burnRoutine);

        ApplyStatusEffect(StatusEffect.StatusEffectType.Burn, true, StatusEffect.StatusEffectCategory.Debuff, duration);
        burnRoutine = StartCoroutine(StartBurning(duration, interval, damage));
    }

    private IEnumerator StartBurning(float duration, float interval, int damage)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            if (health <= 0)
                break;

            TakeDamage(damage);
            yield return new WaitForSeconds(interval);
            timeRemaining -= interval;
        }

        burnRoutine = null;
    }

    public void SetShockRoutine(bool start, float shockInterval, int shockDamage)
    {
        if (start)
            shockRoutine = StartCoroutine(ShockRoutine(shockInterval, shockDamage));
        else
        {
            StopCoroutine(shockRoutine);
            shockRoutine = null;
        }
    }

    public IEnumerator ShockRoutine(float shockInterval, int shockDamage)
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= shockInterval)
            {
                TakeDamage(shockDamage);
                timer = 0;
            }

            yield return null;
        }
    }

    public void ApplyStatusEffect(StatusEffect.StatusEffectType statusEffect, bool haveTimer, StatusEffect.StatusEffectCategory statusEffectCategory, float duration)
    {
        uiController.ApplyStatusEffect(statusEffect, haveTimer, statusEffectCategory, duration);
    }

    public void RemoveStatusEffect(StatusEffect.StatusEffectType statusEffect)
    {
        uiController.RemoveStatusEffect(statusEffect);
    }
}
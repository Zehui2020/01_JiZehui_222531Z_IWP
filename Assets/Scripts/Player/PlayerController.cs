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
    private DeathController deathController;
    private PostEffectController postEffectController;
    [SerializeField] private KnifeController knifeController;

    [SerializeField] private LayerMask enemyLayer;
    private Enemy enemyToLookAt;

    public Transform raycastHitPos;

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
    private Coroutine sprintRoutine;
    private Coroutine dieRoutine;
    private Coroutine passiveHealingRoutine;

    private int waveObjectiveCounter = 0;

    [SerializeField] private VehiclePartSpawner vehiclePartSpawner;
    [SerializeField] private GameObject crudeKnifeRadius;

    [SerializeField] private int itemSpawnInterval = 3;

    public List<VehiclePart.VehiclePartType> vehicleParts = new List<VehiclePart.VehiclePartType>();

    private void OnDisable()
    {
        OnUpdatePoints = null;
    }

    private void Awake()
    {
        Instance = this;
        itemStats.ResetStats();

        // Get player components
        movementController = GetComponent<MovementController>();
        weaponController = GetComponent<WeaponController>();
        uiController = GetComponent<UIController>();
        cameraController = GetComponent<CameraController>();
        itemManager = GetComponent<ItemManager>();
        deathController = GetComponent<DeathController>();
        postEffectController = GetComponent<PostEffectController>();
        playerRB = GetComponent<Rigidbody>();

        // Initialize components
        movementController.IntializeMovementController();
        weaponController.InitWeaponController();
        cameraController.InitCameraController();
        knifeController.InitKnifeController();
        uiController.InitUIController();
        itemManager.InitItemManager();
        deathController.InitDeathController();

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        passiveHealingRoutine = StartCoroutine(PassiveHealing());
    }

    private void Start()
    {
        OnUpdatePoints?.Invoke(points);
        CompanionManager.Instance.ShowMessages(CompanionManager.Instance.companionMessenger.introMessages);

        CompanionManager.Instance.OnMessageFinish += () =>
        {
            EnemySpawner.Instance.StartWave(2f);
            SetWaveObjectiveToSpawnVehiclePart();
        };

        LevelManager.Instance.FadeIn();
    }

    public void SetWaveObjectiveToSpawnVehiclePart()
    {
        waveObjectiveCounter++;

        WaveObjective objective = new WaveObjective(
            Objective.ObjectiveType.Normal, 
            "Survive for " + itemSpawnInterval + " waves " + Utility.Instance.ToRoman(waveObjectiveCounter) + " (0/" + itemSpawnInterval + ")",
            itemSpawnInterval);

        ObjectiveManager.Instance.AddObjective(objective);
        objective.OnObjectiveComplete += vehiclePartSpawner.SpawnVehiclePart;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (health <= maxHealth * 0.2f)
            postEffectController.Pulse(20, 2f, false);
        else
            postEffectController.Pulse(0.5f, 2f, true);

        if (health <= maxHealth * 0.1f)
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.lowHealthMessages);

        if (vehicleParts.Contains(VehiclePart.VehiclePartType.Gas_Tank) && damage > 3)
            BurnPlayer(5f, 1f, 2);

        StopCoroutine(passiveHealingRoutine);
        passiveHealingRoutine = StartCoroutine(PassiveHealing());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ConsoleManager.Instance.SetConsole();

        if (Input.GetKeyDown(KeyCode.Return))
            ConsoleManager.Instance.OnInputCommand();

        if (health <= 0)
        {
            if (dieRoutine == null)
            {
                StopAllCoroutines();
                dieRoutine = StartCoroutine(DieRoutine());
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
            CompanionManager.Instance.SkipMessage();

        if (isDisabled || Time.timeScale == 0)
            return;
            
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        movementController.CheckGroundCollision();
        movementController.CheckOnSlope();
        movementController.HandleMovment(horizontal, vertical);

        if (Input.GetKeyUp(KeyCode.Space))
            movementController.HandleJump();

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
            {
                collidedInteractable.OnInteract();
                uiController.SetInteractNotification(false);
            }
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

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        weaponController.UpdateCurrentWeapon(horizontal, vertical, mouseX, mouseY, movementController.isGrounded || movementController.isOnSlope);
        weaponController.SetSprinting(movementController.isSprinting);

        uiController.UpdateStaminaBar(movementController.stamina, movementData.maxStamina);
        uiController.UpdateHealthBar(health, maxHealth);

        transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        CheckEnemyToLookAt();

        // Play movement audio
        if (movementController.isMoving)
        {
            if (movementController.isSprinting && (movementController.isGrounded || movementController.isOnSlope))
            {
                AudioManager.Instance.OnlyPlayAfterSoundEnds(Sound.SoundName.Sprint);
                AudioManager.Instance.Stop(Sound.SoundName.Walk);
            }
            else if (!movementController.isSprinting && (movementController.isGrounded || movementController.isOnSlope))
            {
                AudioManager.Instance.OnlyPlayAfterSoundEnds(Sound.SoundName.Walk);
                AudioManager.Instance.Stop(Sound.SoundName.Sprint);
            }
            else
            {
                AudioManager.Instance.Stop(Sound.SoundName.Sprint);
                AudioManager.Instance.Stop(Sound.SoundName.Walk);
            }
        }
        else
        {
            AudioManager.Instance.Stop(Sound.SoundName.Sprint);
            AudioManager.Instance.Stop(Sound.SoundName.Walk);
        }

        // Check for corrupted boots stacking
        if (sprintRoutine == null && 
            movementController.isSprinting && 
            movementController.isMoving &&
            itemStats.bootsDamageModifier > 0)
            sprintRoutine = StartCoroutine(DoSprintRoutine());
        else if (sprintRoutine != null && !movementController.isSprinting)
        {
            StopCoroutine(sprintRoutine);
            sprintRoutine = null;
        }

        // Check for shungite
        if (OnMoveRoutine == null && !movementController.isMoving && itemStats.shungiteHealing > 0)
            OnMoveRoutine = StartCoroutine(OnStopMoving());

        if (movementController.isMoving)
        {
            if (movementController.isSprinting && movementController.isGrounded)
            {
                AudioManager.Instance.OnlyPlayAfterSoundEnds(Sound.SoundName.Sprint);
                AudioManager.Instance.Stop(Sound.SoundName.Walk);
            }
            else if (!movementController.isSprinting && movementController.isGrounded)
            {
                AudioManager.Instance.OnlyPlayAfterSoundEnds(Sound.SoundName.Walk);
                AudioManager.Instance.Stop(Sound.SoundName.Sprint);
            }

            if (OnMoveRoutine != null) StopCoroutine(OnMoveRoutine);
            if (ShungiteHealingRoutine != null) StopCoroutine(ShungiteHealingRoutine);

            OnMoveRoutine = null;
            ShungiteHealingRoutine = null;
            RemoveStatusEffect(StatusEffect.StatusEffectType.ShungiteHealing);
        }
        else if (!movementController.isMoving)
        {
            AudioManager.Instance.Stop(Sound.SoundName.Sprint);
            AudioManager.Instance.Stop(Sound.SoundName.Walk);
        }
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
    }

    private IEnumerator DieRoutine()
    {
        uiController.Die();
        weaponController.HideCurrentWeapon();
        cameraController.Die();
        deathController.Die();

        yield return new WaitForSeconds(3f);

        LevelManager.Instance.LoadScene("MainMenu");
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
        uiController.OnADS(isADS, 0.2f);

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

        uiController.SetAmmoNotification(false);
    }

    private IEnumerator StunEnemyRoutine()
    {
        AudioManager.Instance.PlayOneShot(Sound.SoundName.StunGrenade);
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

    private IEnumerator DoSprintRoutine()
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= itemStats.bootsSprintDuration)
            {

                timer = 0;

                if (powerShot >= itemStats.bootsStackLimit)
                    continue;

                powerShot++;
                ApplyStatusEffect(StatusEffect.StatusEffectType.PowerShot, true, StatusEffect.StatusEffectCategory.Buff, powerShot);
            }

            yield return null;
        }
    }

    public void SetCanADS(bool ads)
    {
        canADS = ads;
    }

    private void UseCurrentWeapon()
    {
        if (!weaponController.UseWeapon())
        {
            if (weaponController.GetCurrentWeapon().ammoCount > 0)
                return;

            if (weaponController.ReloadWeapon())
                OnReload();

            return;
        }

        bool show = weaponController.GetCurrentWeapon().ammoCount <= (weaponController.GetCurrentWeapon().weaponData.ammoPerMag * 0.3f);
        uiController.SetAmmoNotification(show);
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

            if (!interactable.GetInteracted())
                uiController.SetInteractNotification(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            collidedInteractable = null;
            interactable.OnExitRange();
            uiController.SetInteractNotification(false);
        }
    }

    public int GetPoints()
    {
        return points;
    }

    public void AddPoints(int amount)
    {
        // Chance to obtain double points
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.doublePointsChance)
            amount *= 2;

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
        uiController.UpdateWeaponLevel(weaponController.GetCurrentWeapon());
    }

    public bool RestockCurrentWeapon()
    {
        return weaponController.RestockWeapon();
    }

    public void RefillAmmoClip()
    {
        weaponController.RefillAmmoClip();
    }

    public Weapon GetCurrentWeapon()
    {
        return weaponController.GetCurrentWeapon();
    }

    public void ShakeCamera(float intensity, float frequency, float duration)
    {
        cameraController.ShakeCamera(intensity, frequency, duration);
    }

    private IEnumerator OnStopMoving()
    {
        yield return new WaitForSeconds(3f);
        ShungiteHealingRoutine = StartCoroutine(DoShungiteHealing());
        ApplyStatusEffect(StatusEffect.StatusEffectType.ShungiteHealing, false, StatusEffect.StatusEffectCategory.Buff, 0);
    }

    private IEnumerator DoShungiteHealing()
    {
        while (true)
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.Heal);
            Heal((int)(maxHealth * itemStats.shungiteHealing));
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator PassiveHealing()
    {
        yield return new WaitForSeconds(5f);

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

    public void ShowCrudeKnifeRadius()
    {
        crudeKnifeRadius.SetActive(true);
        float radius = itemStats.minDistance * 2;
        crudeKnifeRadius.transform.localScale = new Vector3(radius, radius, radius);
    }

    public void OnInteractStun()
    {
        if (itemStats.jitbStunRadius == 0)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, itemStats.jitbStunRadius);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.StunGrenade);
        foreach (Collider col in colliders)
        {
            if (!Utility.Instance.GetTopmostParent(col.transform).TryGetComponent<Enemy>(out Enemy enemy))
                continue;

            enemy.StunEnemy(itemStats.jitbStunDuration);
        }
    }

    public void TogglePause()
    {
        if (uiController.DisplayPauseMenu())
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            AudioManager.Instance.PauseAllSounds();
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            AudioManager.Instance.UnpauseAllSounds();
        }
    }
}
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

    [SerializeField] private MovementData movementData;
    private Rigidbody playerRB;

    private bool isDisabled = false;
    private bool isADS = false;
    private bool canADS = true;
    private IInteractable collidedInteractable;

    public static event System.Action<int> OnUpdatePoints;
    private Coroutine OnMoveRoutine;
    private Coroutine ShungiteHealingRoutine;

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

        EnemySpawner.Instance.StartWave(5f);
    }

    private void Start()
    {
        OnUpdatePoints?.Invoke(points);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ConsoleManager.Instance.SetConsole();

        if (Input.GetKeyDown(KeyCode.Return))
            ConsoleManager.Instance.OnInputCommand();

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

        Collider[] colliders = Physics.OverlapSphere(transform.position, itemStats.stunGrenadeRadius);
        foreach (Collider col in colliders)
        {
            if (!Utility.Instance.GetTopmostParent(col.transform).TryGetComponent<Enemy>(out Enemy enemy))
                continue;

            enemy.StunEnenmy();
        }
    }

    public void SetCanADS(bool ads)
    {
        canADS = ads;
    }

    private void UseCurrentWeapon()
    {
        if (weaponController.UseWeapon())
            cameraController.ShakeCamera(weaponController.GetWeaponCamShakeAmount(), weaponController.GetWeaponCamShakeDuration());
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

    public bool RestockCurrentWeapon()
    {
        return weaponController.RestockWeapon();
    }

    public void RefillAmmoClip()
    {
        weaponController.RefillAmmoClip();
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
}
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

    [SerializeField] private ItemStats itemStats;
    [SerializeField] private MovementData movementData;
    private Rigidbody playerRB;

    private bool isDisabled = false;
    private bool isADS = false;

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

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;

        itemStats.ResetStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisabled)
            return;
            
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        movementController.HandleMovment(horizontal, vertical);

        if (Input.GetKeyUp(KeyCode.Space))
            movementController.HandleJump();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            movementController.ToggleCrouch();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementController.ToggleSprint();

        if (Input.GetKeyDown(KeyCode.R))
            weaponController.ReloadWeapon();

        if (Input.GetKeyDown(KeyCode.E))
            weaponController.SwitchWeapon();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (knifeController.Knife())
                weaponController.HideCurrentWeapon();
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

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))
        {
            isADS = !isADS;
            weaponController.ADSWeapon();
            uiController.OnADS(isADS);

            if (isADS)
                cameraController.Zoom(weaponController.GetWeaponZoomAmount(), weaponController.GetWeaponZoomDuration());
            else
                cameraController.Zoom(60, weaponController.GetWeaponZoomDuration());
        }

        weaponController.UpdateCurrentWeapon(horizontal, vertical, mouseX, mouseY, movementController.isGrounded);

        uiController.UpdateStaminaBar(movementController.stamina, movementData.maxStamina);
        uiController.UpdateHealthBar(health, maxHealth);

        transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
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
        uiController.OnPickupItem(item, itemManager.itemList);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (movementController != null)
            movementController.EnterCollision(col);
    }

    private void OnCollisionExit(Collision col)
    {
        movementController.ExitCollision(col);
    }

    public void AddPoints(int amount)
    {
        points += amount;
    }

    public void RemovePoints(int amount)
    {
        points -= amount;
    }

    public Vector3 GetVelocity()
    {
        return playerRB.velocity;
    }

    public void ShowCurrentWeapon()
    {
        weaponController.ShowCurrentWeapon();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerStats
{
    public static PlayerController Instance;
    private MovementController movementController;
    private WeaponController weaponController;
    private AnimationManager animationManager;
    private UIController uiController;
    private CameraController cameraController;

    [SerializeField] private MovementData movementData;
    private Rigidbody playerRB;

    private bool isDisabled = false;
    private bool isADS = false;

    private void Awake()
    {
        Instance = this;

        // Get player components
        animationManager = GetComponent<AnimationManager>();
        movementController = GetComponent<MovementController>();
        weaponController = GetComponent<WeaponController>();
        uiController = GetComponent<UIController>();
        cameraController = GetComponent<CameraController>();
        playerRB = GetComponent<Rigidbody>();

        // Initialize components
        animationManager.InitAnimationManager();
        movementController.IntializeMovementController();
        weaponController.InitWeaponController();
        cameraController.InitCameraController();

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
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

        if (Input.GetMouseButtonDown(0))
        {
            if (weaponController.UseWeapon())
                cameraController.ShakeCamera(weaponController.GetWeaponCamShakeAmount(), weaponController.GetWeaponCamShakeDuration());
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

        movementController.UpdateAnimation();
        weaponController.UpdateCurrentWeapon(horizontal, vertical, mouseX, mouseY, movementController.isGrounded);

        uiController.UpdateStaminaBar(movementController.stamina, movementData.maxStamina);

        transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void FixedUpdate()
    {
        movementController.MovePlayer();
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

    private void OnCollisionEnter(Collision col)
    {
        if (movementController != null)
            movementController.EnterCollision(col);
    }

    private void OnCollisionExit(Collision col)
    {
        movementController.ExitCollision(col);
    }

    public Vector3 GetVelocity()
    {
        return playerRB.velocity;
    }
}
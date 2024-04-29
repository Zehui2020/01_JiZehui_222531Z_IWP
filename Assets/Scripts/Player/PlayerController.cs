using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private WeaponController weaponController;

    private bool isDisabled = false;

    private void Awake()
    {
        // Get player components
        movementController = GetComponent<MovementController>();
        weaponController = GetComponent<WeaponController>();

        // Initialize components
        movementController.IntializeMovementController();

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisabled)
            return;

        movementController.HandleMovment();

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
            weaponController.UseWeapon();

        movementController.UpdateAnimation();

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
}
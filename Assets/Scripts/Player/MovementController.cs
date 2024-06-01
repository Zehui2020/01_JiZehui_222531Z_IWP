using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;
    [SerializeField] private ItemStats itemStats;
    [SerializeField] private LayerMask groundLayer;
    private CapsuleCollider playerCol;
    private Rigidbody playerRB;

    public bool isMoving = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    public bool isGrounded = true;

    private bool canSprint = true;

    private Vector3 direction;
    public float stamina = 100;
    private float moveSpeed;
    private bool useStamina = true;
    private bool canJump = true;
    private RaycastHit slopeHit;

    // Start is called before the first frame update
    public void IntializeMovementController()
    {
        moveSpeed = movementData.walkSpeed;
        playerCol = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
    }

    public void SetUseStamina(bool isUsing)
    {
        useStamina = isUsing;
        if (!useStamina)
            stamina = 100;
    }

    public void ToggleSprint()
    {
        if (!isMoving || !isGrounded || isCrouching || !canSprint)
            return;

        isSprinting = !isSprinting;

        if (isSprinting)
        {
            moveSpeed = movementData.sprintSpeed * itemStats.sprintSpeedModifier;

            //AudioManager.Instance.Stop("Walk");
        }
        else
        {
            moveSpeed = movementData.walkSpeed;

            //AudioManager.Instance.Stop("Sprint");
        }
    }

    public void SetCanSprint(bool sprint)
    {
        canSprint = sprint;
        if (!sprint)
        {
            isSprinting = sprint;
            moveSpeed = movementData.walkSpeed;
        }
    }

    public void ToggleCrouch()
    {
        if (!isGrounded)
            return;

        isCrouching = !isCrouching;
        isSprinting = false;

        if (isCrouching)
        {
            //AudioManager.Instance.Stop("Sprint");

            moveSpeed = movementData.crouchSpeed;
            playerCol.height = 0.8f;
            playerCol.center = new Vector3(playerCol.center.x, 0.4f, playerCol.center.z);
            playerRB.AddForce(new Vector3(0, 1, 0) * -10f, ForceMode.Impulse);
        }
        else
        {
            moveSpeed = movementData.walkSpeed;
            playerCol.center = new Vector3(playerCol.center.x, 0.9f, playerCol.center.z);
            playerCol.height = 1.78f;
        }
    }

    public void HandleMovment(float horizontal, float vertical)
    {
        isMoving = horizontal != 0 || vertical != 0;

        if (isMoving)
        {
            direction = Camera.main.transform.forward.normalized;

            Vector3 forwardDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward * vertical, Vector3.up);
            Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * horizontal, Vector3.up);
            direction = (forwardDirection + sideDirection).normalized;

            // If player is walking
            //if (!isSprinting && isGrounded)
            //    AudioManager.Instance.OnlyPlayAfterSoundEnds("Walk");
            // If player is sprinting and is moving
            if (isSprinting && playerRB.velocity.magnitude > 0.1f && useStamina && canSprint)
            {
                stamina -= movementData.sprintStaminaCost * Time.deltaTime;

                if (isGrounded)
                {
                    //AudioManager.Instance.Stop("Walk");
                    //AudioManager.Instance.OnlyPlayAfterSoundEnds("Sprint");
                }
            }
        }

        else if (!isMoving && isGrounded)
        {
            playerRB.velocity = Vector3.zero;

            // Disable sprinting if stop moving
            isSprinting = false;

            // Reset move speed
            if (!isCrouching)
                moveSpeed = movementData.walkSpeed;
            else if (isCrouching)
                moveSpeed = movementData.crouchSpeed;
        }

        //if (!isMoving || !isGrounded)
        //{
        //    AudioManager.Instance.Stop("Walk");
        //    AudioManager.Instance.Stop("Sprint");
        //}

        // If run out of stamina
        if (stamina <= 0f && isSprinting)
        {
            stamina = 0f;
            ToggleSprint();
        }

        // Recharge stamina
        if (!isSprinting && stamina < movementData.maxStamina)
            stamina += movementData.staminaRegenRate * Time.deltaTime;

        // Update facing direction
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), moveSpeed * Time.deltaTime);
    }

    public void HandleJump()
    {
        if (!canJump || !isGrounded || isCrouching || stamina < movementData.jumpStaminaCost)
            return;

        canJump = false;
        isGrounded = false;

        // Calculate stamina cost
        float totalJumpCost = movementData.jumpStaminaCost * 0.75f;
        if (totalJumpCost < movementData.jumpStaminaCost)
            totalJumpCost = movementData.jumpStaminaCost;
        if (useStamina)
            stamina -= totalJumpCost;

        playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
        playerRB.AddForce(transform.up * movementData.baseJumpForce, ForceMode.Impulse);

        //AudioManager.Instance.Play("Jump");
    }

    public void MovePlayer()
    {
        if (!isMoving)
            return;

        Vector3 force;
        // Adjust drag & force
        if (isGrounded)
            force = direction * moveSpeed * 10f;
        else if (!isGrounded)
            force = direction * moveSpeed * 10f * movementData.airMultiplier;
        else
            force = Vector3.zero;

        // Move player
        if (OnSlope())
            playerRB.AddForce(GetSlopeMoveDir() * moveSpeed * 10f, ForceMode.Force);
        else
            playerRB.AddForce(force, ForceMode.Force);

        SpeedControl();
    }

    public void CheckGroundCollision()
    {
        RaycastHit groundHit;
        if (!Physics.Raycast(transform.position, Vector3.down, out groundHit, 100, groundLayer))
            return;

        float dist = Vector3.Distance(transform.position, groundHit.point);
        if (dist <= movementData.minGroundDist)
        {
            isGrounded = true;
            canJump = true;
            playerRB.drag = movementData.groundDrag;
        }
        else if (dist > movementData.minGroundDist)
        {
            isGrounded = false;
            canJump = false;
            playerRB.drag = 0;
        }
    }

    public void StopPlayer()
    {
        isMoving = false;
        playerRB.velocity = Vector3.zero;
    }

    private void SpeedControl()
    {
        Vector3 currentVel = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);

        if (OnSlope())
        {
            if (playerRB.velocity.magnitude > movementData.walkSpeed)
                playerRB.velocity = playerRB.velocity.normalized * movementData.walkSpeed;
        }
        else if (currentVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = currentVel.normalized * moveSpeed;
            playerRB.velocity = new Vector3(limitVel.x, playerRB.velocity.y, limitVel.z);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 100))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < movementData.maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;
    [SerializeField] private ItemStats itemStats;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPosition;
    private CapsuleCollider playerCol;
    private Rigidbody playerRB;

    public bool isMoving = false;
    public bool isSprinting = false;
    public bool isGrounded = true;
    public bool isOnSlope = false;

    private bool canSprint = true;

    private Vector3 direction;
    public float stamina = 100;
    private float moveSpeed;
    private bool useStamina = true;
    private bool canJump = true;
    private RaycastHit slopeHit;

    private float moveSpeedModifier = 1;
    private float staminaModifier = 1;

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
        if (!isMoving || !isGrounded || !canSprint)
            return;

        isSprinting = !isSprinting;

        if (isSprinting)
            moveSpeed = movementData.sprintSpeed * itemStats.sprintSpeedModifier;
        else
            moveSpeed = movementData.walkSpeed;
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

    public void HandleMovment(float horizontal, float vertical)
    {
        isMoving = horizontal != 0 || vertical != 0;

        if (isMoving)
        {
            direction = Camera.main.transform.forward.normalized;

            Vector3 forwardDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward * vertical, Vector3.up);
            Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * horizontal, Vector3.up);
            direction = (forwardDirection + sideDirection).normalized;

            if (isSprinting && playerRB.velocity.magnitude > 0.1f && useStamina && canSprint)
                stamina -= movementData.sprintStaminaCost * staminaModifier * Time.deltaTime;
        }

        else if (!isMoving && isGrounded)
        {
            playerRB.velocity = Vector3.zero;
            isSprinting = false;
            moveSpeed = movementData.walkSpeed;
        }

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
        if (!canJump || stamina < movementData.jumpStaminaCost * staminaModifier)
            return;

        if (!isGrounded && !isOnSlope)
            return;

        canJump = false;
        isGrounded = false;

        // Calculate stamina cost
        float totalJumpCost = movementData.jumpStaminaCost * 0.75f;
        if (totalJumpCost < movementData.jumpStaminaCost)
            totalJumpCost = movementData.jumpStaminaCost;
        if (useStamina)
            stamina -= totalJumpCost * staminaModifier;

        playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
        playerRB.AddForce(transform.up * movementData.baseJumpForce, ForceMode.Impulse);

        AudioManager.Instance.Play(Sound.SoundName.Jump);
    }

    public void MovePlayer()
    {
        if (!isMoving)
            return;

        Vector3 force;

        // Adjust drag & force
        if (isGrounded)
            force = direction * moveSpeed * 10f;
        else if (!isGrounded && !canJump)
            force = direction * moveSpeed * 10f * movementData.airMultiplier * moveSpeedModifier;
        else
            force = Vector3.zero;

        // Move player
        if (isOnSlope)
            playerRB.AddForce(GetSlopeMoveDir() * moveSpeed * 10f * moveSpeedModifier, ForceMode.Force);
        else
            playerRB.AddForce(force * moveSpeedModifier, ForceMode.Force);

        SpeedControl();
    }

    public void CheckGroundCollision()
    {
        RaycastHit groundHit;
        if (!Physics.Raycast(groundCheckPosition.position, Vector3.down, out groundHit, 100, groundLayer))
            return;

        float dist = Vector3.Distance(groundCheckPosition.position, groundHit.point);
        if (dist <= movementData.minGroundDist)
        {
            if (!isGrounded && !isOnSlope && playerRB.velocity.y <= -1.5f)
                AudioManager.Instance.OnlyPlayAfterSoundEnds(Sound.SoundName.Land);

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

    public void SetMoveSpeedModifier(float newModifier)
    {
        moveSpeedModifier = newModifier;
    }

    public void SetStaminaModifier(float newModifier)
    {
        staminaModifier = newModifier;
    }

    private void SpeedControl()
    {
        Vector3 currentVel = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);

        if (isOnSlope)
        {
            if (playerRB.velocity.magnitude > movementData.sprintSpeed)
                playerRB.velocity = playerRB.velocity.normalized * movementData.sprintSpeed;
        }
        if (currentVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = currentVel.normalized * moveSpeed;
            playerRB.velocity = new Vector3(limitVel.x, playerRB.velocity.y, limitVel.z);
        }
    }

    public void CheckOnSlope()
    {
        RaycastHit[] hits = Physics.RaycastAll(groundCheckPosition.position, Vector3.down, 100);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Slope"))
            {
                float dist = Vector3.Distance(groundCheckPosition.position, hit.point);
                if (dist <= movementData.minGroundDist)
                {
                    slopeHit = hit;
                    float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    isOnSlope = angle < movementData.maxSlopeAngle && angle != 0;
                    return;
                }
            }
        }

        isOnSlope = false;
    }

    private Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
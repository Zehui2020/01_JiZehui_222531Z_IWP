using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutant : Enemy
{
    public enum MutantState
    {
        CHASE,
        PUNCH,
        SWIPE,
        JUMP_ATTACK,
        ROAR,
        STUN,
        DIE
    }

    public MutantState currentState = MutantState.CHASE;

    public readonly int Run = Animator.StringToHash("MutantRun");
    public readonly int Punch = Animator.StringToHash("MutantPunch");
    public readonly int Swipe = Animator.StringToHash("MutantSwipe");
    public readonly int JumpAttack = Animator.StringToHash("MutantJumpAttack");
    public readonly int Roar = Animator.StringToHash("MutantRoar");
    public readonly int Stun = Animator.StringToHash("MutantStun");

    [SerializeField] private Transform roarPosition;
    [SerializeField] private GroundCrack groundCrack;
    [SerializeField] private float groundCrackLifetime;

    private int punchCounter;
    private Coroutine jumpAttackRoutine;
    private Coroutine roarRoutine;
    private MutantData mutantData;

    private void Start()
    {
        InitEnemy();

        if (enemyData is MutantData)
            mutantData = enemyData as MutantData;
    }

    public void ChangeState(MutantState newState)
    {
        if (stunRoutine != null)
            return;

        currentState = newState;

        switch (newState)
        {
            case MutantState.CHASE:
                if (!CheckChasePlayer())
                    return;

                animator.CrossFade(Run, 0.1f);
                break;

            case MutantState.PUNCH:
                animator.Play(Punch, 0, 0f);
                animator.CrossFade(Punch, 0.1f);
                aiNavigation.StopNavigation();
                break;

            case MutantState.SWIPE:
                animator.Play(Swipe, 0, 0f);
                animator.CrossFade(Swipe, 0.1f);
                aiNavigation.StopNavigation();
                break;

            case MutantState.JUMP_ATTACK:
                animator.Play(JumpAttack, 0, 0f);
                animator.CrossFade(JumpAttack, 0.1f);
                aiNavigation.StopNavigation();
                break;

            case MutantState.ROAR:
                animator.Play(Roar, 0, 0f);
                animator.CrossFade(Roar, 0.1f);
                aiNavigation.StopNavigation();
                break;

            case MutantState.STUN:
                animator.CrossFade(Stun, 0.1f);
                break;

            case MutantState.DIE:
                aiNavigation.StopNavigation();
                animator.enabled = false;
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        if (health <= 0 && currentState != MutantState.DIE)
            ChangeState(MutantState.DIE);

        switch (currentState)
        {
            case MutantState.CHASE:
                CheckChasePlayer();
                break;
            case MutantState.PUNCH:
            case MutantState.SWIPE:
                Vector3 lookDir = (transform.position - player.transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, -lookDir, Time.deltaTime * 10f);
                break;
            default:
                break;
        }
    }

    private bool CheckChasePlayer()
    {
        // Check swipe
        if (punchCounter >= mutantData.punchesTillSwipe && !ChasePlayer(mutantData.swipeRange))
        {
            ChangeState(MutantState.SWIPE);
            punchCounter = 0;
            return false;
        }

        // Check Roar 
        if (roarRoutine == null)
        {
            roarRoutine = StartCoroutine(RoarRoutine());
            return false;
        }

        // Check Jump Attack
        if (jumpAttackRoutine == null && !ChasePlayer(mutantData.jumpAttackRange))
        {
            jumpAttackRoutine = StartCoroutine(JumpAttackRoutine());
            return false;
        }

        // Default to punch
        if (!ChasePlayer(mutantData.punchRange))
        {
            ChangeState(MutantState.PUNCH);
            punchCounter++;
            return false;
        }

        return true;
    }

    public void OnRoar()
    {
        Collider[] cols = Physics.OverlapSphere(roarPosition.position, mutantData.roarRadius);

        foreach (Collider col in cols)
        {
            if (!Utility.Instance.GetTopmostParent(col.transform).TryGetComponent<Enemy>(out Enemy enemy))
                continue;

            enemy.ApplyRoarBuff(mutantData.roarSpeedModifier, mutantData.roarBuffDuration);
        }
    }

    public void OnJumpAttackLand()
    {
        PlayerController.Instance.ShakeCamera(
            mutantData.jumpAttackCamShakeIntensity, 
            mutantData.jumpAttackCamShakeFrequency,
            mutantData.jumpAttackCamShakeDuration);

        Instantiate(groundCrack, transform.position, Quaternion.identity).SetupGroundCrack(groundCrackLifetime, Mathf.FloorToInt(enemyData.damage / 2f));
    }

    private IEnumerator JumpAttackRoutine()
    {
        ChangeState(MutantState.JUMP_ATTACK);

        yield return new WaitForSeconds(mutantData.jumpAttackCooldown);

        jumpAttackRoutine = null;
    }

    private IEnumerator RoarRoutine()
    {
        ChangeState(MutantState.ROAR);

        yield return new WaitForSeconds(mutantData.roarCooldown);

        roarRoutine = null;
    }

    public override IEnumerator OnStun()
    {
        ChangeState(MutantState.STUN);
        aiNavigation.StopNavigation();

        yield return new WaitForSeconds(itemStats.stunGrenadeDuration);

        aiNavigation.ResumeNavigation();
        stunRoutine = null;
        ChangeState(MutantState.CHASE);
    }
}
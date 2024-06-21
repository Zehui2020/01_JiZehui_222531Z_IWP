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

    public void ChangeState(MutantState newState)
    {
        if (stunRoutine != null)
            return;

        currentState = newState;

        switch (newState)
        {
            case MutantState.CHASE:
                if (!ChasePlayer())
                {
                    //ChangeState(MutantState.SPIT);
                    return;
                }
                animator.CrossFade(Run, 0.1f);
                break;

            case MutantState.PUNCH:
                animator.Play(Punch, 0, 0f);
                animator.CrossFade(Punch, 0.1f);
                aiNavigation.StopNavigation();
                break;

            case MutantState.DIE:
                aiNavigation.StopNavigation();
                animator.enabled = false;
                break;

            case MutantState.STUN:
                animator.CrossFade(Stun, 0.1f);
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
                //if (!ChasePlayer())
                    //ChangeState(MutantState.SPIT);
                break;
            default:
                break;
        }
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
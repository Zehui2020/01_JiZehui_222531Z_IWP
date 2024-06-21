using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : Enemy
{
    public enum SpitterState
    {
        CHASE,
        SPIT,
        STUN,
        DIE
    }
    public SpitterState currentState = SpitterState.CHASE;

    public readonly int Run = Animator.StringToHash("ZombieRun");
    public readonly int Spit = Animator.StringToHash("ZombieAttack");
    public readonly int Stun = Animator.StringToHash("ZombieStun");
    public readonly int Die = Animator.StringToHash("ZombieDie");

    public void ChangeState(SpitterState newState)
    {
        if (stunRoutine != null)
            return;

        currentState = newState;

        switch (newState)
        {
            case SpitterState.CHASE:
                if (!ChasePlayer())
                {
                    ChangeState(SpitterState.SPIT);
                    return;
                }
                animator.CrossFade(Run, 0.1f);
                break;

            case SpitterState.SPIT:
                animator.Play(Spit, 0, 0f);
                animator.CrossFade(Spit, 0.1f);
                aiNavigation.StopNavigation();
                break;

            case SpitterState.DIE:
                aiNavigation.StopNavigation();
                animator.enabled = false;
                ragdollController.ActivateRagdoll();
                break;

            case SpitterState.STUN:
                animator.CrossFade(Stun, 0.1f);
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        if (health <= 0 && currentState != SpitterState.DIE)
            ChangeState(SpitterState.DIE);

        switch (currentState)
        {
            case SpitterState.CHASE:
                if (!ChasePlayer())
                    ChangeState(SpitterState.SPIT);
                break;
            default:
                break;
        }
    }

    public override IEnumerator OnStun()
    {
        ChangeState(SpitterState.STUN);
        aiNavigation.StopNavigation();

        yield return new WaitForSeconds(itemStats.stunGrenadeDuration);

        aiNavigation.ResumeNavigation();
        stunRoutine = null;
        ChangeState(SpitterState.CHASE);
    }
}
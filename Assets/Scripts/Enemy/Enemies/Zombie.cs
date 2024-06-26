using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    public enum ZombieState
    {
        SPAWN,
        CHASE,
        ATTACK,
        STUN,
        DIE
    }
    public ZombieState currentState = ZombieState.SPAWN;

    public readonly int Run = Animator.StringToHash("ZombieRun");
    public readonly int Attack = Animator.StringToHash("ZombieAttack");
    public readonly int Stun = Animator.StringToHash("ZombieStun");
    public readonly int Die = Animator.StringToHash("ZombieDie");

    public void ChangeState(ZombieState newState)
    {
        if (stunRoutine != null)
            return;

        currentState = newState;

        switch (newState)
        {
            case ZombieState.CHASE:
                if (!ChasePlayer(enemyData.attackRange))
                {
                    ChangeState(ZombieState.ATTACK);
                    return;
                }
                animator.CrossFade(Run, 0.1f);
                break;

            case ZombieState.ATTACK:
                animator.Play(Attack, 0, 0f);
                animator.CrossFade(Attack, 0.1f);
                aiNavigation.StopNavigation();
                break;

            case ZombieState.DIE:
                aiNavigation.StopNavigation();
                animator.enabled = false;
                break;

            case ZombieState.STUN:
                animator.CrossFade(Stun, 0.1f);
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        if (health <= 0 && currentState != ZombieState.DIE)
            ChangeState(ZombieState.DIE);

        switch (currentState)
        {
            case ZombieState.CHASE:
                if (!ChasePlayer(enemyData.attackRange))
                    ChangeState(ZombieState.ATTACK);
                break;
            default:
                break;
        }
    }

    public override IEnumerator OnStun()
    {
        ChangeState(ZombieState.STUN);
        aiNavigation.StopNavigation();

        yield return new WaitForSeconds(itemStats.stunGrenadeDuration);

        aiNavigation.ResumeNavigation();
        stunRoutine = null;
        ChangeState(ZombieState.CHASE);
    }
}
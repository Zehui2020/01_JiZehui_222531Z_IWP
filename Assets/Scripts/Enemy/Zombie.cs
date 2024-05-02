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
        DIE
    }
    public ZombieState currentState = ZombieState.SPAWN;

    public readonly int Run = Animator.StringToHash("ZombieRun");
    public readonly int Attack = Animator.StringToHash("ZombieAttack");
    public readonly int Die = Animator.StringToHash("ZombieDie");

    public void ChangeState(ZombieState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case ZombieState.CHASE:
                if (!ChasePlayer())
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
                animator.CrossFade(Die, 0.1f);
                enemyCol.enabled = false;
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
                if (!ChasePlayer())
                    ChangeState(ZombieState.ATTACK);
                break;
            default:
                break;
        }
    }
}
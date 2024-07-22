using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Zombie;

public class Charger : Enemy
{
    public enum ChargerState
    {
        CHASE,
        ATTACK,
        STUN,
        DIE
    }
    public ChargerState currentState = ChargerState.CHASE;

    public readonly int Run = Animator.StringToHash("ChargerRun");
    public readonly int Attack = Animator.StringToHash("ChargerAttack");
    public readonly int Stun = Animator.StringToHash("ChargerStun");
    public readonly int Die = Animator.StringToHash("ChargerDie");

    public void ChangeState(ChargerState newState)
    {
        switch (newState)
        {
            case ChargerState.CHASE:
                if (!ChasePlayer(enemyData.attackRange))
                {
                    ChangeState(ChargerState.ATTACK);
                    return;
                }
                animator.CrossFade(Run, 0.1f);
                break;
            case ChargerState.ATTACK:
                animator.Play(Attack, 0, 0f);
                animator.CrossFade(Attack, 0.1f);
                aiNavigation.StopNavigation();
                break;
            case ChargerState.DIE:
                PlayRandomDieSound();
                aiNavigation.StopNavigation();
                animator.enabled = false;
                break;
            case ChargerState.STUN:
                animator.CrossFade(Stun, 0.1f);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (health <= 0 && currentState != ChargerState.DIE)
            ChangeState(ChargerState.DIE);

        switch (currentState)
        {
            case ChargerState.CHASE:
                PlayRandomGrowlSound(2, 5);
                if (!ChasePlayer(enemyData.attackRange))
                    ChangeState(ChargerState.ATTACK);
                break;
            default:
                break;
        }
    }

    public override IEnumerator OnStun(float duration)
    {
        ChangeState(ChargerState.STUN);
        aiNavigation.StopNavigation();

        yield return new WaitForSeconds(duration);

        aiNavigation.ResumeNavigation();
        stunRoutine = null;
        ChangeState(ChargerState.CHASE);
    }
}
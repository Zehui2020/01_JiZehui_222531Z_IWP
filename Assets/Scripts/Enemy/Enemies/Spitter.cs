using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

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

    public readonly int Run = Animator.StringToHash("SpitterRun");
    public readonly int Spit = Animator.StringToHash("SpitterSpit");
    public readonly int Stun = Animator.StringToHash("SpitterStun");

    [SerializeField] private Transform projectileSpawnPos;
    [SerializeField] private float projectileEjectForce;

    private void Start()
    {
        InitEnemy();
    }

    public void ChangeState(SpitterState newState)
    {
        if (stunRoutine != null)
            return;

        currentState = newState;

        switch (newState)
        {
            case SpitterState.CHASE:
                if (CheckSpit())
                    return;
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
                CheckSpit();
                break;
            case SpitterState.SPIT:
                Vector3 lookDir = (projectileSpawnPos.position - player.transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, -lookDir, Time.deltaTime * 10f);
                break;
            default:
                break;
        }
    }

    private bool CheckSpit()
    {
        aiNavigation.SetNavMeshTarget(player.transform.position, enemyData.moveSpeed * speedModifier);

        if (aiNavigation.OnReachTarget(enemyData.attackRange))
        {
            Vector3 dir = (projectileSpawnPos.position - player.transform.position).normalized;
            Physics.Raycast(projectileSpawnPos.position, -dir, out RaycastHit hit, enemyData.attackRange);

            if (hit.collider == null || !hit.collider.CompareTag("Player"))
            {
                aiNavigation.ResumeNavigation();
                return false;
            }

            aiNavigation.StopNavigation();
            ChangeState(SpitterState.SPIT);
            return true;
        }

        aiNavigation.ResumeNavigation();
        return false;
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

    public void SpitProjectile()
    {
        Vector3 shootDir = (projectileSpawnPos.position - player.transform.position).normalized;

        SpitterProjectile projectile = ObjectPool.Instance.GetPooledObject("SpitterProjectile", true).GetComponent<SpitterProjectile>();
        projectile.SetupProjectile(projectileSpawnPos.position, -shootDir, projectileEjectForce, enemyData.damage);
    }
}
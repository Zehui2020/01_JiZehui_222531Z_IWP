using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyStats
{
    public enum EnemyType { Normal, Elite, MiniBoss, Boss }
    public EnemyType enemyType;

    [SerializeField] protected EnemyData enemyData;
    protected AINavigation aiNavigation;
    protected GameObject player;
    protected Animator animator;
    [SerializeField] protected Collider[] enemyCols;
    private CombatCollisionController collisionController;

    [SerializeField] private ParticleSystemEmitter firePS;

    private Coroutine burnRoutine;

    public virtual void InitEnemy()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        collisionController = GetComponent<CombatCollisionController>();
        aiNavigation = GetComponent<AINavigation>();
        aiNavigation.InitNavMeshAgent();
    }

    public void SpawnEnemy(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        gameObject.SetActive(true);
    }

    public bool ChasePlayer()
    {
        aiNavigation.SetNavMeshTarget(player.transform.position, enemyData.moveSpeed);

        if (aiNavigation.OnReachTarget(enemyData.attackRange))
        {
            aiNavigation.StopNavigation();
            return false;
        }
        else
        {
            aiNavigation.ResumeNavigation();
            return true;
        }
    }

    public void BurnEnemy(float duration, float interval, int damage)
    {
        if (burnRoutine != null)
            StopCoroutine(burnRoutine);

        firePS.PlayLoopingPS();
        burnRoutine = StartCoroutine(StartBurning(duration, interval, damage));
    }

    private IEnumerator StartBurning(float duration, float interval, int damage)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            TakeDamage(damage, Vector3.zero, DamagePopup.ColorType.WHITE);
            yield return new WaitForSeconds(interval);
            timeRemaining -= interval;
        }

        firePS.StopLoopingPS();
        burnRoutine = null;
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public void OnDamageEventStart(int col)
    {
        collisionController.EnableCollider(col);
        collisionController.StartDamageCheck(enemyData.damage);
    }

    public void OnDamageEventEnd(int col)
    {
        collisionController.DisableCollider(col);
        collisionController.StopDamageCheck();
    }

    protected void SetEnemyColliders(bool active)
    {
        foreach (Collider col in enemyCols)
            col.enabled = active;
    }
}
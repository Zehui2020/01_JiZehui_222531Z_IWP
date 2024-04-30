using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Stats
{
    public enum EnemyType { Normal, Elite, MiniBoss, Boss }
    public EnemyType enemyType;

    [SerializeField] protected EnemyData enemyData;
    protected AINavigation aiNavigation;
    protected GameObject player;
    protected Animator animator;
    private CombatCollisionController collisionController;

    private void Awake()
    {
        InitEnemy();
    }

    public virtual void InitEnemy()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        collisionController = GetComponent<CombatCollisionController>();
        aiNavigation = GetComponent<AINavigation>();
        aiNavigation.InitNavMeshAgent();
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
}
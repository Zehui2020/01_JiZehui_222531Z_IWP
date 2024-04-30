using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Normal, Elite, MiniBoss, Boss }
    public EnemyType enemyType;

    private AINavigation aiNavigation;
    private GameObject player;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        InitEnemy();
    }

    public virtual void InitEnemy()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiNavigation = GetComponent<AINavigation>();
        aiNavigation.InitNavMeshAgent();

    }

    // Update is called once per frame
    void Update()
    {
        aiNavigation.SetNavMeshTarget(player.transform.position, moveSpeed);
        if (aiNavigation.OnReachTarget(player.transform.position, 5f))
            aiNavigation.StopNavigation();
        else
            aiNavigation.ResumeNavigation();
    }
}
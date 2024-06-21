using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float moveSpeed;
    public float attackRange;
    public int damage;
    public float deathPushbackForce;
}
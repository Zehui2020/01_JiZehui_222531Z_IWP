using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform[] spawnPoints;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            EnemySpawner.Instance.SetCurrentRoom(this);
    }
}
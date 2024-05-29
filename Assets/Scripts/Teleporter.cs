using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination;

    public void Teleport(Transform target)
    {
        target.position = teleportDestination.position;
    }

    public void OnTriggerEnter(Collider col)
    {
        Teleport(col.transform);
    }
}
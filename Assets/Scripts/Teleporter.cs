using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform teleportLocation;

    public void Teleport()
    {
        PlayerController.Instance.transform.position = teleportLocation.position;
        PlayerController.Instance.transform.forward = teleportLocation.forward;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floodlights : VehiclePart
{
    [SerializeField] private float shockInterval;
    [SerializeField] private int shockDamage;

    public override void OnInteract()
    {
        base.OnInteract();
        PlayerController.Instance.SetShockRoutine(true, shockInterval, shockDamage);
    }
}
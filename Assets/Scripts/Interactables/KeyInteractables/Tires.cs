using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tires : VehiclePart
{
    [SerializeField] private float tireMovementSpeedModifier;

    public override void OnInteract()
    {
        base.OnInteract();
        PlayerController.Instance.SetMoveSpeedModifier(tireMovementSpeedModifier);
    }
}

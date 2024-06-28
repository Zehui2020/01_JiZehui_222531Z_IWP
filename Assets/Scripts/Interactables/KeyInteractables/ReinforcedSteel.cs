using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcedSteel : VehiclePart
{
    [SerializeField] private float steelStaminaModifier;

    public override void OnInteract()
    {
        base.OnInteract();
        PlayerController.Instance.SetStaminaModifier(steelStaminaModifier);
    }
}
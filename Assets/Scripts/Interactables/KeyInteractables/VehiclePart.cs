using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VehiclePart : MonoBehaviour, IInteractable
{
    public enum VehiclePartType
    {
        Floodlights,
        Gas_Tank,
        Reinforced_Steel,
        Engine,
        Tires
    }
    public VehiclePartType vehiclePartType;

    public string vehiclePartName;

    public event Action OnInteractEvent;

    public void InitInteractable()
    {
        OnInteractEvent += PlayerController.Instance.OnInteractStun;
    }

    public virtual void OnEnterRange()
    {
    }

    public virtual void OnExitRange()
    {
    }

    public virtual void OnInteract()
    {
        switch (vehiclePartType)
        {
            case VehiclePartType.Floodlights:
                PlayerController.Instance.ApplyStatusEffect(StatusEffect.StatusEffectType.Floodlight, false, StatusEffect.StatusEffectCategory.VehiclePart, 0);
                break;
            case VehiclePartType.Gas_Tank:
                PlayerController.Instance.ApplyStatusEffect(StatusEffect.StatusEffectType.GasTank, false, StatusEffect.StatusEffectCategory.VehiclePart, 0);
                break;
            case VehiclePartType.Reinforced_Steel:
                PlayerController.Instance.ApplyStatusEffect(StatusEffect.StatusEffectType.ReinforcedSteel, false, StatusEffect.StatusEffectCategory.VehiclePart, 0);
                break;
            case VehiclePartType.Tires:
                PlayerController.Instance.ApplyStatusEffect(StatusEffect.StatusEffectType.Tires, false, StatusEffect.StatusEffectCategory.VehiclePart, 0);
                break;
        }

        OnInteractEvent?.Invoke();
        PlayerController.Instance.AddVehiclePart(this);

        CompanionManager.Instance.ShowVehiclePartPickupMessage(this);

        Destroy(gameObject);
    }

    public void SetCost(int newCost)
    {
    }
}
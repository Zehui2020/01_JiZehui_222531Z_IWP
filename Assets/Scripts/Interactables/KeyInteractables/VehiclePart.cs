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

    [SerializeField] protected int vehiclePartCost;
    [SerializeField] protected TextMeshProUGUI cost;
    public string vehiclePartName;

    public event Action OnInteractEvent;

    public void InitInteractable()
    {
        cost.text = vehiclePartCost.ToString() + "P";
        cost.gameObject.SetActive(false);
    }

    public virtual void OnEnterRange()
    {
        cost.gameObject.SetActive(true);
    }

    public virtual void OnExitRange()
    {
        cost.gameObject.SetActive(false);
    }

    public virtual void OnInteract()
    {
        if (PlayerController.Instance.GetPoints() < vehiclePartCost)
        {
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.interactionFailMessages);
            return;
        }

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
        cost.gameObject.SetActive(false);
        PlayerController.Instance.DeductPoints(vehiclePartCost);
        PlayerController.Instance.AddVehiclePart(this);

        CompanionManager.Instance.ShowVehiclePartPickupMessage(this);

        Destroy(gameObject);
    }

    public void SetCost(int newCost)
    {
        vehiclePartCost = newCost;
        cost.text = vehiclePartCost.ToString() + "P";
    }
}
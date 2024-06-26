using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour, IInteractable
{
    public void InitInteractable()
    {
    }

    public void OnEnterRange()
    {
    }

    public void OnExitRange()
    {
    }

    public void OnInteract()
    {
        if (PlayerController.Instance.vehicleParts.Count == 0)
            return;

        switch (PlayerController.Instance.vehicleParts[0])
        {
            case VehiclePart.VehiclePartType.Floodlights:
                break;
            case VehiclePart.VehiclePartType.Gas_Tank:
                break;
            case VehiclePart.VehiclePartType.Reinforced_Steel:
                break;
            case VehiclePart.VehiclePartType.Engine:
                break;
            case VehiclePart.VehiclePartType.Tires:
                break;
        }
    }

    public void SetCost(int newCost)
    {
    }
}
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

    [SerializeField] private int vehiclePartCost;
    [SerializeField] private TextMeshProUGUI cost;

    public void InitInteractable()
    {
        cost.text = vehiclePartCost.ToString() + "P";
        cost.gameObject.SetActive(false);
    }

    public void OnEnterRange()
    {
        cost.gameObject.SetActive(true);
    }

    public void OnExitRange()
    {
        cost.gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        PlayerController.Instance.AddVehiclePart(this);
        Destroy(gameObject);
    }

    public void SetCost(int newCost)
    {
        vehiclePartCost = newCost;
        cost.text = vehiclePartCost.ToString() + "P";
    }
}
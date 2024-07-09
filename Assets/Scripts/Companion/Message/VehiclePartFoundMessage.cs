using UnityEngine;

[CreateAssetMenu(menuName = "Companion/VehiclePartFoundMessage")]
public class VehiclePartFoundMessage : CompanionMessage
{
    public VehiclePart.VehiclePartType vehiclePartType;
}
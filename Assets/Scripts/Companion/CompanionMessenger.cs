using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Companion/CompanionMessenger")]
public class CompanionMessenger : ScriptableObject
{
    public CompanionMessage[] introMessages;
    public CompanionMessage[] interactionFailMessages;
    public CompanionMessage[] lowHealthMessages;
    public CompanionMessage bossSpawnMessage;
    public WeaponPickupMessage[] weaponPickupMessages;
    public VehiclePartFoundMessage[] vehiclePartFoundMessages;
    public CompanionMessage[] vehiclePartPickupMessages;
}
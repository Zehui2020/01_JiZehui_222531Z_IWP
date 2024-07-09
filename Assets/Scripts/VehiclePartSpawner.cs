using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePartSpawner : MonoBehaviour
{
    [SerializeField] private List<VehiclePart> vehicleParts = new List<VehiclePart>();
    [SerializeField] private Transform[] spawnPoints;
    private int currentVehiclePart = 0;

    public void SpawnVehiclePart()
    {
        int randSpawnPoint = Random.Range(0, spawnPoints.Length);

        VehiclePart vehiclePart = Instantiate(vehicleParts[currentVehiclePart], spawnPoints[randSpawnPoint].position, Quaternion.identity);
        vehiclePart.InitInteractable();

        CompanionManager.Instance.ShowVehiclePartFoundMessage(vehiclePart.vehiclePartType);

        currentVehiclePart++;
        if (currentVehiclePart >= vehicleParts.Count)
            currentVehiclePart = vehicleParts.Count - 1;
    }
}
public class VehicleToTruckObjective : Objective
{
    private VehiclePart.VehiclePartType vehiclePartType;

    public VehicleToTruckObjective(ObjectiveType objectiveType, string objectiveName, VehiclePart.VehiclePartType vehiclePartType) : base(objectiveType, objectiveName)
    {
        this.objectiveType = objectiveType;
        this.objectiveName = objectiveName;
        this.vehiclePartType = vehiclePartType;

        EndingVehicle.OnInteractEvent += CompleteObjective;
    }

    public void CompleteObjective(VehiclePart.VehiclePartType vehiclePartType)
    {
        if (this.vehiclePartType != vehiclePartType)
            return;

        CompleteObjective();
    }
}
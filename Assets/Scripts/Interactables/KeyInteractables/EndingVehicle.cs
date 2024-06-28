using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject radiusPrefab;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;

    [SerializeField] private float fixRadius;
    [SerializeField] private float fixDuration;

    [SerializeField] private GameObject floodlights;
    [SerializeField] private GameObject gasTank;
    [SerializeField] private GameObject reinforcedSteel;
    [SerializeField] private GameObject tires;

    private Coroutine FixRoutine;
    private Objective vehicleObjective;
    public List<VehiclePart.VehiclePartType> vehicleParts = new List<VehiclePart.VehiclePartType>();

    public void InitInteractable()
    {
    }

    public void OnEnterRange()
    {
    }

    public void OnExitRange()
    {
    }

    public void SetCost(int newCost)
    {
    }

    private void CreateObjective()
    {
        switch (PlayerController.Instance.vehicleParts[0])
        {
            case VehiclePart.VehiclePartType.Floodlights:
                vehicleObjective = new Objective(Objective.ObjectiveType.Progress, "Attach the Floodlights");
                break;
            case VehiclePart.VehiclePartType.Gas_Tank:
                vehicleObjective = new Objective(Objective.ObjectiveType.Progress, "Attach the Gas Tank");
                break;
            case VehiclePart.VehiclePartType.Reinforced_Steel:
                vehicleObjective = new Objective(Objective.ObjectiveType.Progress, "Reinforce the vehicle");
                break;
            case VehiclePart.VehiclePartType.Engine:
                vehicleObjective = new Objective(Objective.ObjectiveType.Progress, "Install the Engine");
                break;
            case VehiclePart.VehiclePartType.Tires:
                vehicleObjective = new Objective(Objective.ObjectiveType.Progress, "Attach the Tires");
                break;
        }

        if (vehicleObjective != null)
            ObjectiveManager.Instance.AddObjective(vehicleObjective);
    }

    public void OnInteract()
    {
        // WIN GAME CHECK
        if (vehicleParts.Count >= 5)
        {
            Debug.Log("WWIN");
            return;
            // WIN GAME!!!!!
        }

        if (PlayerController.Instance.vehicleParts.Count == 0 || FixRoutine != null)
            return;

        CreateObjective();
        FixRoutine = StartCoroutine(CheckRadiusRoutine(fixRadius, fixDuration));
    }

    private IEnumerator CheckRadiusRoutine(float radius, float duration)
    {
        GameObject radiusGO = Instantiate(radiusPrefab, transform.position, Quaternion.identity);
        radiusGO.transform.localScale = new Vector3(radius, radius, radius);

        ProgressObjectiveUI objectiveUI = ObjectiveManager.Instance.GetObjectiveUI(vehicleObjective) as ProgressObjectiveUI;

        float timer = 0;
        while (timer < duration)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, radius / 2f, playerLayer);

            if (cols.Length == 0)
                yield return null;
            else
            {
                timer += Time.deltaTime;
                objectiveUI.UpdateProgressBar(timer, duration);
            }

            yield return null;
        }

        OnCompleteFix();
        PlayerController.Instance.vehicleParts.RemoveAt(0);
        Destroy(radiusGO);

        FixRoutine = null;
    }

    private void OnCompleteFix()
    {
        vehicleParts.Add(PlayerController.Instance.vehicleParts[0]);
        ObjectiveManager.Instance.RemoveObjective(vehicleObjective);

        switch (PlayerController.Instance.vehicleParts[0])
        {
            case VehiclePart.VehiclePartType.Floodlights:
                PlayerController.Instance.SetShockRoutine(false, 0, 0);
                PlayerController.Instance.RemoveStatusEffect(StatusEffect.StatusEffectType.Floodlight);
                floodlights.SetActive(true);
                break;
            case VehiclePart.VehiclePartType.Gas_Tank:
                PlayerController.Instance.RemoveStatusEffect(StatusEffect.StatusEffectType.GasTank);
                gasTank.SetActive(true);
                break;
            case VehiclePart.VehiclePartType.Reinforced_Steel:
                PlayerController.Instance.SetStaminaModifier(1);
                PlayerController.Instance.RemoveStatusEffect(StatusEffect.StatusEffectType.ReinforcedSteel);
                reinforcedSteel.SetActive(true);
                break;
            case VehiclePart.VehiclePartType.Engine:
                break;
            case VehiclePart.VehiclePartType.Tires:
                PlayerController.Instance.SetMoveSpeedModifier(1);
                PlayerController.Instance.RemoveStatusEffect(StatusEffect.StatusEffectType.Tires);
                tires.SetActive(true);
                animator.SetTrigger("tires");
                break;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : VehiclePart
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float fixRadius;
    [SerializeField] private float fixDuration;
    [SerializeField] private GameObject radiusPrefab;
    [SerializeField] private Animator animator;

    private GameObject radiusGO;
    private Coroutine fixRoutine = null;
    private Objective engineObjective;

    public override void OnInteract()
    {
        if (isInteracted)
            return;

        isInteracted = true;

        base.OnInteract();

        engineObjective = new Objective(Objective.ObjectiveType.Progress, "Assemble the Engine");
        ObjectiveManager.Instance.AddObjective(engineObjective);
        engineObjective.OnObjectiveComplete += () => { CompanionManager.Instance.ShowVehiclePartPickupMessage(this); };

        animator.SetTrigger("fix");
        fixRoutine = StartCoroutine(CheckRadiusRoutine());
    }

    private IEnumerator CheckRadiusRoutine()
    {
        radiusGO = Instantiate(radiusPrefab, transform.position, Quaternion.identity);
        radiusGO.transform.localScale = new Vector3(fixRadius, fixRadius, fixRadius);

        ProgressObjectiveUI objectiveUI = engineObjective.objectiveUI as ProgressObjectiveUI;

        float timer = 0;
        while (timer < fixDuration)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, fixRadius / 2f, playerLayer);

            if (cols.Length == 0)
            {
                animator.speed = 0;
                yield return null;
            }
            else
            {
                timer += Time.deltaTime;
                objectiveUI.UpdateProgressBar(timer, fixDuration);
                animator.speed = 1;
            }

            yield return null;
        }

        ObjectiveManager.Instance.RemoveObjective(engineObjective);
    }

    public override void OnEnterRange()
    {
        if (fixRoutine == null)
            base.OnEnterRange();
    }

    public void OnFinishedFixing()
    {
        StopCoroutine(fixRoutine);
        Destroy(radiusGO);
        PlayerController.Instance.AddVehiclePart(this);
        Destroy(gameObject);
    }
}

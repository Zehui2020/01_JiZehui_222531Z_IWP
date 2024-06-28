using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [SerializeField] private ObjectiveUI normalObjective;
    [SerializeField] private ObjectiveUI progressObjective;

    [SerializeField] private Dictionary<Objective, ObjectiveUI> currentObjectives = new Dictionary<Objective, ObjectiveUI>();
    [SerializeField] private Transform objectiveUIParent;

    private void Awake()
    {
        Instance = this;
    }

    public void AddObjective(Objective objective)
    {
        ObjectiveUI objectiveUI = null;

        switch (objective.objectiveType)
        {
            case Objective.ObjectiveType.Normal:
                objectiveUI = Instantiate(normalObjective);
                break;
            case Objective.ObjectiveType.Progress:
                objectiveUI = Instantiate(progressObjective);
                break;
        }

        objectiveUI.transform.SetParent(objectiveUIParent, false);
        objectiveUI.SetupObjective(objective);
        currentObjectives.Add(objective, objectiveUI);
    }

    public void RemoveObjective(Objective objective)
    {
        if (!currentObjectives.TryGetValue(objective, out ObjectiveUI objectiveUI))
            return;

        currentObjectives.Remove(objective);
        objectiveUI.RemoveObjective();
    }

    public ObjectiveUI GetObjectiveUI(Objective objective)
    {
        if (currentObjectives.TryGetValue(objective, out ObjectiveUI objectiveUI))
            return objectiveUI;

        return null;
    }

    public void OnObjectiveComplete()
    {

    }
}
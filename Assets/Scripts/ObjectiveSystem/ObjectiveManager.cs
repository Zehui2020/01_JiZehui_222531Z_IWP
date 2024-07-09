using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [SerializeField] private ObjectiveUI normalObjective;
    [SerializeField] private ObjectiveUI progressObjective;

    [SerializeField] private List<Objective> currentObjectives = new List<Objective>();
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

        objective.SetObjectiveUI(objectiveUI);
        objectiveUI.transform.SetParent(objectiveUIParent, false);
        objectiveUI.SetObjectiveName(objective.objectiveName);

        currentObjectives.Add(objective);
    }

    public void RemoveObjective(Objective objective)
    {
        Objective foundObjective = currentObjectives.Find(o => o == objective);

        if (foundObjective == null)
            return;

        foundObjective.CompleteObjective();
        currentObjectives.Remove(foundObjective);
    }
}
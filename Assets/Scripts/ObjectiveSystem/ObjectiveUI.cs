using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveName;
    [HideInInspector] public Objective objective;
    [SerializeField] private Animator animator;

    public void SetupObjective(Objective objective)
    {
        objectiveName.text = objective.objectiveName;
        this.objective = objective;
    }

    public void RemoveObjective()
    {
        animator.SetTrigger("remove");
    }

    public void DestroyObjective()
    {
        Destroy(gameObject);
    }
}
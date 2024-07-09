using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveName;
    [SerializeField] private Animator animator;

    public void SetObjectiveName(string objName)
    {
        objectiveName.text = objName;
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
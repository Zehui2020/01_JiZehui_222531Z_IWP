using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableTrigger : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteractEvent;

    public void InitInteractable()
    {
    }

    public void SetCost(int newCost)
    {
    }

    public void OnInteract()
    {
        OnInteractEvent?.Invoke();
    }

    public void OnEnterRange()
    {
    }

    public void OnExitRange()
    {
    }

    public bool GetInteracted()
    {
        return false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AmmoShelf : MonoBehaviour, IInteractable
{
    [SerializeField] private int shelfCost;
    [SerializeField] private TextMeshProUGUI cost;

    public event Action OnInteractEvent;

    public void InitInteractable()
    {
        cost.text = shelfCost.ToString() + "P";
        cost.gameObject.SetActive(false);
    }

    public void OnEnterRange()
    {
        cost.gameObject.SetActive(true);
    }

    public void OnExitRange()
    {
        cost.gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        if (PlayerController.Instance.GetPoints() < shelfCost)
        {
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.interactionFailMessages);
            return;
        }

        OnInteractEvent?.Invoke();
        if (PlayerController.Instance.RestockCurrentWeapon())
            PlayerController.Instance.DeductPoints(shelfCost);
    }

    public void SetCost(int newCost)
    {
        shelfCost = newCost;
        cost.text = shelfCost.ToString() + "P";
    }
}
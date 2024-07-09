using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackAPunch : MonoBehaviour, IInteractable
{
    [SerializeField] private int packAPunchCost;
    [SerializeField] private TextMeshProUGUI cost;

    public event Action OnInteractEvent;

    public void InitInteractable()
    {
        cost.text = packAPunchCost.ToString() + "P";
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
        if (PlayerController.Instance.GetPoints() < packAPunchCost)
        {
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.interactionFailMessages);
            return;
        }

        PlayerController.Instance.DeductPoints(packAPunchCost);
        PlayerController.Instance.UpgradeCurrentWeapon();
        OnInteractEvent?.Invoke();
    }

    public void SetCost(int newCost)
    {
        packAPunchCost = newCost;
        cost.text = packAPunchCost.ToString() + "P";
    }
}
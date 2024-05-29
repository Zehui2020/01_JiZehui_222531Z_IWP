using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private int doorCost;
    [SerializeField] private TextMeshProUGUI cost;

    public void InitInteractable()
    {
        cost.text = doorCost.ToString() + "P";
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
        if (PlayerController.Instance.GetPoints() < doorCost)
            return;

        PlayerController.Instance.DeductPoints(doorCost);
        gameObject.SetActive(false);
    }

    public void SetCost(int newCost)
    {
        doorCost = newCost;
        cost.text = doorCost.ToString() + "P";
    }
}

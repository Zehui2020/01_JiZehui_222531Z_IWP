using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoShelf : MonoBehaviour, IInteractable
{
    [SerializeField] private int shelfCost;
    [SerializeField] private TextMeshProUGUI cost;

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
            return;

        if (PlayerController.Instance.RestockCurrentWeapon())
            PlayerController.Instance.DeductPoints(shelfCost);
    }

    public void SetCost(int newCost)
    {
        shelfCost = newCost;
        cost.text = shelfCost.ToString() + "P";
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackAPunch : MonoBehaviour, IInteractable
{
    [SerializeField] private int packAPunchCost;
    [SerializeField] private TextMeshProUGUI cost;

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
            return;

        PlayerController.Instance.DeductPoints(packAPunchCost);
        PlayerController.Instance.UpgradeCurrentWeapon();
    }

    public void SetCost(int newCost)
    {
        packAPunchCost = newCost;
        cost.text = packAPunchCost.ToString() + "P";
    }
}
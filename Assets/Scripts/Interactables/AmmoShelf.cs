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

    private void Start()
    {
        InitInteractable();
    }

    public void InitInteractable()
    {
        cost.text = shelfCost.ToString() + "P";
        cost.gameObject.SetActive(false);
        OnInteractEvent += PlayerController.Instance.OnInteractStun;
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
            AudioManager.Instance.PlayOneShot(Sound.SoundName.InteractFail);
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.interactionFailMessages);
            return;
        }

        if (PlayerController.Instance.RestockCurrentWeapon())
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.XKillDrum);
            PlayerController.Instance.RefillAmmoClip();
            PlayerController.Instance.DeductPoints(shelfCost);
            OnInteractEvent?.Invoke();
        }
        else
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.InteractFail);
        }
    }

    public void SetCost(int newCost)
    {
        shelfCost = newCost;
        cost.text = shelfCost.ToString() + "P";
    }

    public bool GetInteracted()
    {
        return false;
    }
}
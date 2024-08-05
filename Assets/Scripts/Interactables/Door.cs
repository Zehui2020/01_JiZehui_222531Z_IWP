using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private int doorCost;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider doorCollider;
    [SerializeField] private AudioSource audioSource;
    private bool isOpened = false;

    public event Action OnInteractEvent;

    private void Start()
    {
        InitInteractable();
    }

    public void InitInteractable()
    {
        cost.text = doorCost.ToString() + "P";
        cost.gameObject.SetActive(false);
        OnInteractEvent += PlayerController.Instance.OnInteractStun;
    }

    public void OnEnterRange()
    {
        if (isOpened)
            return;

        cost.gameObject.SetActive(true);
    }

    public void OnExitRange()
    {
        cost.gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        if (isOpened)
            return;

        if (PlayerController.Instance.GetPoints() < doorCost)
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.InteractFail);
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.interactionFailMessages);
            return;
        }

        isOpened = true;
        doorCollider.enabled = false;
        PlayerController.Instance.DeductPoints(doorCost);
        animator.SetTrigger("open");
        cost.gameObject.SetActive(false);
        OnInteractEvent?.Invoke();

        if (audioSource != null)
            audioSource.PlayOneShot(AudioManager.Instance.FindSound(Sound.SoundName.DoorOpen).clip);
    }

    public void SetCost(int newCost)
    {
        doorCost = newCost;
        cost.text = doorCost.ToString() + "P";
    }

    public bool GetInteracted()
    {
        return isOpened;
    }
}

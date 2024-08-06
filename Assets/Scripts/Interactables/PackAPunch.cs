using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackAPunch : MonoBehaviour, IInteractable
{
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private int packAPunchCost;
    [SerializeField] private List<int> tierCosts;
    [SerializeField] private int costIncreasePerLevel;

    private bool isInRange = false;

    public event Action OnInteractEvent;
    private Coroutine interactRoutine;

    private void Start()
    {
        InitInteractable();
    }

    public void InitInteractable()
    {
        cost.text = packAPunchCost.ToString() + "P";
        cost.gameObject.SetActive(false);
        OnInteractEvent += PlayerController.Instance.OnInteractStun;
    }

    private void Update()
    {
        if (cost.IsActive())
        {
            CalculateCost();
            cost.text = packAPunchCost.ToString() + "P";
        }
    }

    public void OnEnterRange()
    {
        if (interactRoutine == null)
            cost.gameObject.SetActive(true);

        isInRange = true;
    }

    public void OnExitRange()
    {
        cost.gameObject.SetActive(false);
        isInRange = false;
    }

    public void OnInteract()
    {
        if (interactRoutine != null)
            return;

        if (PlayerController.Instance.GetPoints() < packAPunchCost)
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.InteractFail);
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.interactionFailMessages);
        }
        else
            interactRoutine = StartCoroutine(InteractRoutine());
    }

    private IEnumerator InteractRoutine()
    {
        cost.gameObject.SetActive(false);
        PlayerController.Instance.DeductPoints(packAPunchCost);
        PlayerController.Instance.UpgradeCurrentWeapon();
        audioSource.PlayOneShot(AudioManager.Instance.FindSound(Sound.SoundName.PackAPunchInteract).clip);
        OnInteractEvent?.Invoke();

        yield return new WaitForSeconds(6f);

        if (isInRange)
            cost.gameObject.SetActive(true);

        interactRoutine = null;
    }

    public void SetCost(int newCost)
    {
        packAPunchCost = newCost;
        cost.text = packAPunchCost.ToString() + "P";
    }

    private void CalculateCost()
    {
        Weapon currentWeapon = PlayerController.Instance.GetCurrentWeapon();
        if (currentWeapon.level - 1 >= tierCosts.Count)
            return;

        SetCost(tierCosts[currentWeapon.level - 1]);
    }

    public bool GetInteracted()
    {
        return (interactRoutine == null && PlayerController.Instance.points >= packAPunchCost) ? false : true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;
using TMPro;
using static Sound;

public class Chest : PooledObject, IInteractable
{
    public enum ChestType
    {
        Normal,
        Large,
        Weapon,
        Special
    }
    public ChestType chestType;
    public Item.ItemCatagory itemCatagory;

    [SerializeField] private int chestCost;
    [SerializeField] private Light spotLight;
    [SerializeField] private bool isStartingChest;
    [SerializeField] private string closeAnimState;
    public bool isOpened = false;
    private bool hasSetLight = false;

    [SerializeField] private Transform itemParent;
    [SerializeField] private TextMeshProUGUI cost;

    private ItemPickup itemPickup;
    private WeaponPickup weaponPickup;
    private Animator animator;
    private AudioSource audioSource;

    public event System.Action OnInteractEvent;

    public override void Init()
    {
        InitInteractable();
    }

    public void InitInteractable()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        cost.text = chestCost.ToString() + "P";
        cost.gameObject.SetActive(false);
        hasSetLight = false;

        OnInteractEvent += PlayerController.Instance.OnInteractStun;
    }

    public void OnInteract()
    {
        if (isOpened && (itemPickup != null || weaponPickup != null))
        {
            itemPickup?.PickupItem();
            itemPickup = null;

            weaponPickup?.PickupWeapon();
            weaponPickup = null;

            SetLights(0);
            hasSetLight = true;

            if (isStartingChest)
                Destroy(gameObject, 3f);

            return;
        }

        if (isOpened)
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.InteractFail);
            return;
        }

        if (PlayerController.Instance.GetPoints() < chestCost)
        {
            CompanionManager.Instance.ShowRandomMessage(CompanionManager.Instance.companionMessenger.interactionFailMessages);
            AudioManager.Instance.PlayOneShot(Sound.SoundName.InteractFail);
            return;
        }

        isOpened = true;
        animator.SetTrigger("open");
        PlayerController.Instance.DeductPoints(chestCost);
        cost.gameObject.SetActive(false);

        OnInteractEvent?.Invoke();

        if (chestType == ChestType.Weapon)
            AudioManager.Instance.PlayOneShot(audioSource, SoundName.WeaponChestOpen);
        else
            AudioManager.Instance.PlayOneShot(audioSource, SoundName.NormalChestOpen);

        switch (chestType)
        {
            case ChestType.Normal:
                int randNum = Random.Range(0, 100);
                if (randNum > 20)
                    DisplayItem(null, 67, 25, 8);
                else
                    DisplayWeapon();
                return;
            case ChestType.Large:
                DisplayItem(null, 0, 50, 50);
                return;
            case ChestType.Weapon:
                DisplayWeapon();
                return;
        }

        DisplayItem(itemCatagory, 67, 25, 8);
    }

    private void DisplayItem(Item.ItemCatagory? itemCatagory, float commonRarity, float uncommonRarity, float legenaryRarity)
    {
        itemPickup = ChestManager.Instance.GetItemPickup(itemCatagory, commonRarity, uncommonRarity, legenaryRarity);
        itemPickup.transform.SetParent(itemParent);
        itemPickup.transform.localPosition = Vector3.zero;
    }

    private void DisplayWeapon()
    {
        weaponPickup = ObjectPool.Instance.GetPooledObject(PlayerController.Instance.GetRandomWeapon().weaponData.weapon.ToString(), true).GetComponent<WeaponPickup>();
        weaponPickup.transform.SetParent(itemParent);
        weaponPickup.transform.localPosition = Vector3.zero;
        weaponPickup.transform.forward = transform.forward;
    }

    public void SetLights(int active)
    {
        if (hasSetLight)
            return;

        if (active == 0)
            spotLight.enabled = false;
        else
            spotLight.enabled = true;
    }

    public void OnEnterRange()
    {
        if (!isOpened)
            cost.gameObject.SetActive(true);
    }

    public void OnExitRange()
    {
        if (!isOpened)
            cost.gameObject.SetActive(false);
    }

    public void SetCost(int newCost)
    {
        chestCost = newCost;
        if (chestCost <= 0)
            cost.text = "FREE!";
        else
            cost.text = chestCost.ToString() + "P";
    }

    public void ResetChest()
    {
        animator.SetTrigger("close");

        isOpened = false;
        cost.gameObject.SetActive(false);
        SetLights(1);
        hasSetLight = false;

        if (weaponPickup != null)
        {
            weaponPickup.Release();
            weaponPickup.gameObject.SetActive(false);
        }
        if (itemPickup != null)
        {
            itemPickup.Release();
            itemPickup.gameObject.SetActive(false);
        }

        Release();
        gameObject.SetActive(false);
    }

    public bool GetInteracted()
    {
        return (!isOpened && PlayerController.Instance.points >= chestCost) ? false : true;
    }

    private void OnDisable()
    {
        OnInteractEvent = null;
    }
}
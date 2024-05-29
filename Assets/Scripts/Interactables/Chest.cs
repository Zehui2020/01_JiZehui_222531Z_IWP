using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;
using TMPro;

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

    [SerializeField] private Transform itemParent;
    [SerializeField] private TextMeshProUGUI cost;
    private ItemPickup itemPickup;
    private WeaponPickup weaponPickup;
    private Animator animator;

    public event System.Action OnInteractEvent;

    public override void Init()
    {
        InitInteractable();
    }

    public void InitInteractable()
    {
        animator = GetComponent<Animator>();
        cost.text = chestCost.ToString() + "P";
        cost.gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        if (PlayerController.Instance.GetPoints() < chestCost)
            return;

        if (isOpened && (itemPickup != null || weaponPickup != null))
        {
            itemPickup?.PickupItem();
            itemPickup = null;

            weaponPickup?.PickupWeapon();
            weaponPickup = null;

            if (isStartingChest)
                Destroy(gameObject);

            return;
        }

        if (isOpened)
            return;

        isOpened = true;
        animator.SetTrigger("open");
        PlayerController.Instance.DeductPoints(chestCost);
        cost.gameObject.SetActive(false);

        OnInteractEvent?.Invoke();

        switch (chestType)
        {
            case ChestType.Normal:
                int randNum = Random.Range(0, 100);
                if (randNum > 20)
                    DisplayItem(null, 73, 25, 2);
                else
                    DisplayWeapon();
                return;
            case ChestType.Large:
                DisplayItem(null, 0, 80, 20);
                return;
            case ChestType.Weapon:
                DisplayWeapon();
                return;
        }

        DisplayItem(itemCatagory, 73, 25, 2);
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
        cost.text = chestCost.ToString() + "P";
    }

    public void ResetChest()
    {
        animator.Play(Animator.StringToHash(closeAnimState), 0, 0);

        isOpened = false;
        cost.gameObject.SetActive(false);

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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private GameObject crossHair;
    [SerializeField] private ItemPickupAlert itemPickupAlert;

    [SerializeField] private ItemUI itemUIPrefab;
    [SerializeField] private Transform itemUIParent;
    private List<ItemUI> itemUIs = new List<ItemUI>();

    [SerializeField] private TextMeshProUGUI waveAlertText;
    private Animator waveAlertAnimator;
    [SerializeField] private TextMeshProUGUI waveNumberText;

    [SerializeField] private WeaponUI mainWeaponUI;
    [SerializeField] private WeaponUI secondaryWeaponUI;

    [SerializeField] private TextMeshProUGUI pointText;

    public void InitUIController()
    {
        waveAlertAnimator = waveAlertText.GetComponent<Animator>();

        EnemySpawner.WaveStarted += OnWaveStart;
        EnemySpawner.WaveEnded += OnWaveEnd;
        PlayerController.OnUpdatePoints += UpdatePointCount;
    }

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = currentStamina;
        staminaBar.maxValue = maxStamina;
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBar.value = currentHealth;
        healthBar.maxValue = maxHealth;
        healthText.text = currentHealth + " / " + maxHealth;
    }

    public void OnPickupItem(Item item)
    {
        itemPickupAlert.DisplayAlert(item);

        foreach (ItemUI itemUI in itemUIs)
        {
            if (itemUI.item.itemType == item.itemType)
            {
                itemUI.AddItemCount();
                return;
            }
        }

        ItemUI newItemUI = Instantiate(itemUIPrefab, itemUIParent);
        newItemUI.SetupItemUI(item);
        itemUIs.Add(newItemUI);
    }

    public void OnADS(bool isADS)
    {
        crossHair.SetActive(!isADS);
    }

    public void OnWaveStart(int waveNumber)
    {
        waveNumberText.text = waveNumber.ToString();
        waveAlertText.text = "Wave " + waveNumber;

        waveAlertAnimator.SetTrigger("show");
    }

    public void OnWaveEnd()
    {
        waveAlertText.text = "Wave Cleared";
        waveAlertAnimator.SetTrigger("show");
    }

    public void SetWeaponCount(int weaponCount)
    {
        if (weaponCount < 2)
            secondaryWeaponUI.gameObject.SetActive(false);
        else
            secondaryWeaponUI.gameObject.SetActive(true);
    }

    public void OnReplaceWeapon(Weapon newWeapon)
    {
        mainWeaponUI.SetWeaponUI(newWeapon);
    }

    public void SetWeaponUIs(Weapon mainWeapon, Weapon secondaryWeapon)
    {
        mainWeaponUI.SetWeaponUI(mainWeapon);

        if (secondaryWeapon != null)
            secondaryWeaponUI.SetWeaponUI(secondaryWeapon);
    }

    public void UpdateAmmoCount(Weapon weapon)
    {
        mainWeaponUI.UpdateAmmoCount(weapon.ammoCount, weapon.totalAmmo);
    }

    public void UpdatePointCount(int points)
    {
        pointText.text = points.ToString() + "P";
    }
}
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

    public void InitUIController()
    {
        waveAlertAnimator = waveAlertText.GetComponent<Animator>();

        EnemySpawner.WaveStarted += OnWaveStart;
        EnemySpawner.WaveEnded += OnWaveEnd;
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

    public void OnPickupItem(Item item, List<Item> itemList)
    {
        itemPickupAlert.DisplayAlert(item);

        if (!FindItemInList(item))
        {
            ItemUI itemUI = Instantiate(itemUIPrefab, itemUIParent);
            itemUI.SetupItemUI(item);
            itemUIs.Add(itemUI);
        }
    }

    public void OnADS(bool isADS)
    {
        crossHair.SetActive(!isADS);
    }

    public bool FindItemInList(Item item)
    {
        foreach (ItemUI itemUI in itemUIs)
        {
            if (itemUI.item.itemType != item.itemType)
                continue;

            itemUI.AddItemCount();
            return true;
        }

        return false;
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
}
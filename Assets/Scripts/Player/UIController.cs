using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject deathCanvas;

    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private float unADSCrosshairWidth;
    [SerializeField] private float ADSCrosshairWidth;

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

    [SerializeField] private Transform statusParent;
    [SerializeField] private StatusEffectUI statusEffectUI;
    [SerializeField] private int maxStatusEffects;
    private List<StatusEffectUI> statusEffectUIs = new List<StatusEffectUI>();

    [SerializeField] private TextMeshProUGUI fpsCounter;
    float deltaTime = 0;

    [SerializeField] private RectTransform crosshairRect;
    [SerializeField] private Image crosshair;

    private Coroutine crosshairLerpRoutine;

    public void InitUIController()
    {
        waveAlertAnimator = waveAlertText.GetComponent<Animator>();

        EnemySpawner.WaveStarted += OnWaveStart;
        EnemySpawner.WaveEnded += OnWaveEnd;
        PlayerController.OnUpdatePoints += UpdatePointCount;
        crosshairRect.sizeDelta = new Vector2(unADSCrosshairWidth, unADSCrosshairWidth);

        StartCoroutine(UpdateFPS());
    }

    private IEnumerator UpdateFPS()
    {
        float fps = 0;

        while (true)
        {
            fps = 1.0f / deltaTime;
            fpsCounter.text = "FPS: " + Mathf.Ceil(fps).ToString();

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
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

    public void UpdateCrosshair(Sprite newCrosshair)
    {
        crosshair.sprite = newCrosshair;
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

    public void OnADS(bool isADS, float duration)
    {
        if (crosshairLerpRoutine != null)
            StopCoroutine(crosshairLerpRoutine);

        if (isADS)
            crosshairLerpRoutine = StartCoroutine(LerpCrosshairSize(ADSCrosshairWidth, duration));
        else
            crosshairLerpRoutine = StartCoroutine(LerpCrosshairSize(unADSCrosshairWidth, duration));
    }

    private IEnumerator LerpCrosshairSize(float targetWidth, float duration)
    {
        float elapsedTime = 0f;
        float initialWidth = crosshairRect.sizeDelta.x;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / duration);
            float newWidth = Mathf.Lerp(initialWidth, targetWidth, lerpFactor);
            crosshairRect.sizeDelta = new Vector2(newWidth, newWidth);
            yield return null;
        }

        crosshairRect.sizeDelta = new Vector2(targetWidth, targetWidth);
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

    public void UpdateWeaponLevel(Weapon weapon)
    {
        mainWeaponUI.SetWeaponUI(weapon);
    }

    public void UpdateAmmoCount(Weapon weapon)
    {
        mainWeaponUI.UpdateAmmoCount(weapon.ammoCount, weapon.totalAmmo);
    }

    public void UpdatePointCount(int points)
    {
        pointText.text = points.ToString() + "P";
    }

    public void RemoveStatusEffect(StatusEffect.StatusEffectType statusEffect)
    {
        for (int i = 0; i < statusEffectUIs.Count; i++)
        {
            if (statusEffectUIs[i].targetStatusEffect.statusEffectType == statusEffect)
                statusEffectUIs[i].RemoveStatus();
        }
    }

    public void ApplyStatusEffect(StatusEffect.StatusEffectType statusEffect, bool haveTimer, StatusEffect.StatusEffectCategory statusEffectCategory, float duration)
    {
        if (statusEffectUIs.Count >= maxStatusEffects)
            return;

        foreach (StatusEffectUI ui in statusEffectUIs)
        {
            if (ui == null) continue;

            if (ui.targetStatusEffect.statusEffectType == statusEffect)
            {
                ui.ResetStatus(duration);
                return;
            }
        }

        StatusEffectUI newEffectUI = Instantiate(statusEffectUI, statusParent);
        newEffectUI.ApplyStatus(statusEffect, haveTimer, statusEffectCategory, duration);
        newEffectUI.OnTimerUp += OnStatusEffectTimerEnd;
        statusEffectUIs.Add(newEffectUI);
    }

    public void OnStatusEffectTimerEnd(StatusEffectUI targetEffectUI)
    {
        for (int i = 0; i < statusEffectUIs.Count; i++)
        {
            if (statusEffectUIs[i].targetStatusEffect.statusEffectType == targetEffectUI.targetStatusEffect.statusEffectType)
            {
                StatusEffectUI ui = statusEffectUIs[i];
                statusEffectUIs.Remove(ui);
                ui.OnTimerUp -= OnStatusEffectTimerEnd;
                Destroy(ui.gameObject);
                return;
            }
        }
    }

    public void Die()
    {
        canvas.gameObject.SetActive(false);
        deathCanvas.gameObject.SetActive(true);
    }
}
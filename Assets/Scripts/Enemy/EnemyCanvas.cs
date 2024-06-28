using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCanvas : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject enemyUI;

    [SerializeField] private Transform statusParent;
    [SerializeField] private StatusEffectUI statusEffectUI;
    [SerializeField] private int maxStatusEffects;
    private List<StatusEffectUI> statusEffectUIs = new List<StatusEffectUI>();

    public void SetHealthBarActive(bool active)
    {
        healthBar.gameObject.SetActive(active);
    }

    public void SetHealthbar(int health, int maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    public void ApplyStatusEffect(StatusEffect.StatusEffectType statusEffect,  bool haveTimer, StatusEffect.StatusEffectCategory statusEffectCategory, float duration)
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

    public void OnEnemyDie()
    {
        for (int i = 0; i < statusEffectUIs.Count; i++)
        {
            statusEffectUIs[i].OnTimerUp -= OnStatusEffectTimerEnd;
            Destroy(statusEffectUIs[i].gameObject);
            statusEffectUIs.Remove(statusEffectUIs[i]);
        }
    }
}
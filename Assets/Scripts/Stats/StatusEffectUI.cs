using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Sprite buffBorder;
    [SerializeField] private Sprite debuffBorder;
    [SerializeField] private Sprite vehiclePartBorder;

    [SerializeField] private Image border;
    [SerializeField] private Image statusIcon;
    [SerializeField] private TextMeshProUGUI timer;

    [SerializeField] private StatusEffect[] allStatusEffects;
    [SerializeField] private Animator animator;

    public StatusEffect targetStatusEffect;
    public event System.Action<StatusEffectUI> OnTimerUp;

    private Coroutine TimerRoutine;

    public void RemoveStatus()
    {
        animator.SetTrigger("destroy");
    }

    public void ApplyStatus(StatusEffect.StatusEffectType statusEffect, bool haveTimer, StatusEffect.StatusEffectCategory statusEffectCategory, float duration)
    {
        foreach (StatusEffect status in allStatusEffects)
        {
            if (status.statusEffectType == statusEffect)
            {
                targetStatusEffect = status;
                break;
            }
        }

        if (targetStatusEffect == null)
            return;

        statusIcon.sprite = targetStatusEffect.icon;

        switch (statusEffectCategory)
        {
            case StatusEffect.StatusEffectCategory.Buff:
                border.sprite = buffBorder;
                break;
            case StatusEffect.StatusEffectCategory.Debuff:
                border.sprite = debuffBorder;
                break;
            case StatusEffect.StatusEffectCategory.VehiclePart:
                border.sprite = vehiclePartBorder;
                break;
        }
            
        if (haveTimer)
            TimerRoutine = StartCoroutine(StartTimer(duration));
        else
            timer.text = "-";
    }

    public void ResetStatus(float duration)
    {
        if (!gameObject)
            return;

        if (TimerRoutine != null)
            StopCoroutine(TimerRoutine);

        TimerRoutine = StartCoroutine(StartTimer(duration));
    }

    private IEnumerator StartTimer(float duration)
    {
        float statusDuration = duration;
        while (statusDuration > 0)
        {
            statusDuration -= Time.deltaTime;
            timer.text = Mathf.RoundToInt(statusDuration).ToString() + "s";

            yield return null;
        }

        timer.text = "0s";
        animator.SetTrigger("destroy");
    }

    public void InvokeDestroyEvent()
    {
        OnTimerUp?.Invoke(this);
    }
}
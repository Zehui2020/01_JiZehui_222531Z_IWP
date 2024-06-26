using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Sprite buffBorder;
    [SerializeField] private Sprite debuffBorder;

    [SerializeField] private Image border;
    [SerializeField] private Image statusIcon;
    [SerializeField] private TextMeshProUGUI timer;

    [SerializeField] private StatusEffect[] allStatusEffects;
    [SerializeField] private Animator animator;

    public StatusEffect targetStatusEffect;
    public event System.Action<StatusEffectUI> OnTimerUp;

    private Coroutine TimerRoutine;

    public void ApplyStatus(StatusEffect.StatusEffectType statusEffect, bool isBuff, float duration)
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

        if (isBuff)
            border.sprite = buffBorder;
        else
            border.sprite = debuffBorder;

        TimerRoutine = StartCoroutine(StartTimer(duration));
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

        animator.SetTrigger("destroy");
    }

    public void InvokeDestroyEvent()
    {
        OnTimerUp?.Invoke(this);
    }
}
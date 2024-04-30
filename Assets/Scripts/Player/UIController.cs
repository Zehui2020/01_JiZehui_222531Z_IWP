using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;
    [SerializeField] private GameObject crossHair;

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = currentStamina;
        staminaBar.maxValue = maxStamina;
        if (currentStamina >= maxStamina)
            staminaBar.gameObject.SetActive(false);
        else
            staminaBar.gameObject.SetActive(true);
    }

    public void OnADS(bool isADS)
    {
        crossHair.SetActive(!isADS);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressObjectiveUI : ObjectiveUI
{
    [SerializeField] private Slider progressBar;

    public void UpdateProgressBar(float currentProgress, float maxProgress)
    {
        progressBar.value = currentProgress;
        progressBar.maxValue = maxProgress;
    }
}
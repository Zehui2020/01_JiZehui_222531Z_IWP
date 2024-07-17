using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInTrigger : MonoBehaviour
{
    private void Start()
    {
        LevelManager.Instance.FadeIn();
    }
}
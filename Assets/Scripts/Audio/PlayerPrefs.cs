using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefs")]
public class PlayerPrefs : ScriptableObject
{
    public float masterVolume;
    public float sfxVolume;
    public float bgmVolume;

    public void ResetVolume()
    {
        masterVolume = 1.0f;
        sfxVolume = 1.0f;
        bgmVolume= 1.0f;
    }
}
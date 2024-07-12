using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;

    public void ResetVolume()
    {
        masterVolume = 1.0f;
        bgmVolume = 1.0f;
        sfxVolume = 1.0f;
    }
}
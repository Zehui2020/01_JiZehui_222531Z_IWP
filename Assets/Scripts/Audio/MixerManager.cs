using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    // Audio
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    // Player prefs
    [SerializeField] private PlayerPrefs playerPrefs;

    private void Start()
    {
        SetSliders();
    }

    public void SetMasterVolume()
    {
        SetVolume("Master", Mathf.Log10(masterSlider.value) * 20);
        playerPrefs.masterVolume = masterSlider.value;
    }

    public void SetBGMVolume()
    {
        SetVolume("BGM", Mathf.Log10(bgmSlider.value) * 20 + 5);
        playerPrefs.bgmVolume = bgmSlider.value;
    }

    public void SetSFXVolume()
    {
        SetVolume("SFX", Mathf.Log10(sfxSlider.value) * 20);
        playerPrefs.sfxVolume = sfxSlider.value;
    }

    public void ResetVolume()
    {
        playerPrefs.ResetVolume();

        masterSlider.value = playerPrefs.masterVolume;
        bgmSlider.value = playerPrefs.bgmVolume;
        sfxSlider.value = playerPrefs.sfxVolume;

        SetMasterVolume();
        SetBGMVolume();
        SetSFXVolume();
    }

    public void SetSliders()
    {
        masterSlider.value = playerPrefs.masterVolume;
        bgmSlider.value = playerPrefs.bgmVolume;
        sfxSlider.value = playerPrefs.sfxVolume;
    }

    private void SetVolume(string name, float volume)
    {
        audioMixer.SetFloat(name, volume);
    }

    private void OnApplicationQuit()
    {
        ResetVolume();
    }
}
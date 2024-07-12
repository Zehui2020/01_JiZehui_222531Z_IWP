using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class Sound
{
    public enum SoundName
    {
        PistolShell,
        PistolShoot,
        PistolWield,
        PistolReload,
        ShotgunShoot,
        ShotgunWield,
        ShotgunReload,
        ShotgunPump,
        ShotgunShell,
        DryFire,
        LMGShoot,
        LMGReload,
        LMGWield,
        FlamethrowerShoot,
        FlamethrowerReload,
        FlamethrowerWield,
        FlamethrowerDryFire,
        GLauncherShoot,
        GLauncherWield,
        GLauncherStartReload,
        GLauncherReloadOnce,
        GLauncherEndReload,
        SawnOffShoot,
        SawnOffWield,
        SawnOffReload,
        AKShoot,
        AKReload,
        AKWield,
        MorseCode,
        KnifeFire,
        WeaponHide,
        WeaponShow,
        RifleShoot,
        RifleReload,
        RifleWield,
        GrenadeExplode,
        Jump,
        Land,
        Walk,
        Sprint,
        NormalChestOpen,
        WeaponChestOpen,
        StunGrenade,
        DynamiteExplode,
        Block,
        Heal,
        XKillDrum
    }
    public SoundName name;

    public AudioClip clip;

    public AudioMixerGroup mixerGroup;

    [Range(0.1f, 3f)]
    public float pitch = 1;

    [Range(0f, 10f)]
    public float volume = 1;

    [HideInInspector]
    public AudioSource source;

    public bool loop;
    public bool createSource = true;

    public Coroutine fadeRoutine;

    public IEnumerator FadeSoundRoutine(bool fadeIn, float duration, float targetVolume)
    {
        if (source == null)
        {
            fadeRoutine = null;
            yield break;
        }

        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            if (!fadeIn)
                source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            else
                source.volume = Mathf.Lerp(0, targetVolume, time / duration);
            yield return null;
        }

        source.Stop();
        source.volume = volume;
        fadeRoutine = null;

        yield break;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostEffectController : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private float pulseMinVignetteValue = 0f;
    [SerializeField] private float pulseMaxVignetteValue = 0.2f;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;

    private Coroutine pulseRoutine;


    void Start()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chromaticAberration);
    }

    public void Pulse(float duration, float speed, bool pulseOnce)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        if (pulseOnce)
            pulseRoutine = StartCoroutine(PulseOnce(duration, speed));
        else
            pulseRoutine = StartCoroutine(PulseRoutine(duration, speed));
    }

    private IEnumerator PulseRoutine(float duration, float speed)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = Mathf.PingPong(Time.time * speed, 1);
            vignette.intensity.value = Mathf.Lerp(pulseMinVignetteValue, pulseMaxVignetteValue, t);
            chromaticAberration.intensity.value = Mathf.Lerp(0, 1, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        vignette.intensity.value = pulseMinVignetteValue;
        chromaticAberration.intensity.value = 0;
    }

    private IEnumerator PulseOnce(float duration, float speed)
    {
        float halfDuration = duration / 2;
        float elapsedTime = 0;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            vignette.intensity.value = Mathf.Lerp(pulseMinVignetteValue, pulseMaxVignetteValue, t);
            chromaticAberration.intensity.value = Mathf.Lerp(0, 0.3f, t);
            yield return null;
        }

        elapsedTime = 0;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            vignette.intensity.value = Mathf.Lerp(pulseMaxVignetteValue, pulseMinVignetteValue, t);
            chromaticAberration.intensity.value = Mathf.Lerp(0.3f, 0, t);
            yield return null;
        }

        vignette.intensity.value = pulseMinVignetteValue;
        pulseRoutine = null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachinePerlin;

    private Coroutine shakeRoutine;
    private Coroutine zoomRoutine;

    public void InitCameraController()
    {
        cinemachinePerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float frequency, float timer)
    {
        if (shakeRoutine == null)
            shakeRoutine = StartCoroutine(StartShakeCamera(intensity, frequency, timer));
    }

    public void ApplyRecoil(float recoilX, float recoilY)
    {
        cinemachineVirtualCamera.GetComponent<RecoilExtension>().ApplyRecoil(recoilX, recoilY);
    }

    public void SetADS(bool isADS)
    {
        cinemachineVirtualCamera.GetComponent<SensitivityExtension>().OnToggleADS(isADS);
    }

    private IEnumerator StartShakeCamera(float intensity, float frequency, float timer)
    {
        float duration = timer;
        float elapsedTime = 0f;

        cinemachinePerlin.m_AmplitudeGain = intensity;
        cinemachinePerlin.m_FrequencyGain = frequency;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / duration);
            cinemachinePerlin.m_AmplitudeGain = Mathf.Lerp(intensity, 0f, lerpFactor);
            cinemachinePerlin.m_FrequencyGain = Mathf.Lerp(frequency, 0f, lerpFactor);

            yield return null;
        }

        cinemachinePerlin.m_AmplitudeGain = 0f;
        cinemachinePerlin.m_FrequencyGain = 0;
        shakeRoutine = null;
    }

    public void Zoom(float zoomAmount, float zoomDuration)
    {
        if (zoomRoutine != null)
        {
            StopCoroutine(zoomRoutine);
            zoomRoutine = null;
        }

        zoomRoutine = StartCoroutine(StartZoom(zoomAmount, zoomDuration));
    }

    private IEnumerator StartZoom(float zoomAmount, float zoomDuration)
    {
        float duration = zoomDuration;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / duration);
            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, zoomAmount, lerpFactor);

            yield return null;
        }

        cinemachineVirtualCamera.m_Lens.FieldOfView = zoomAmount;
        zoomRoutine = null;
    }
}
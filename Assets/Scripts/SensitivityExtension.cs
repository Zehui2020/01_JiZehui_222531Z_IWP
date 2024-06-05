using UnityEngine;
using Cinemachine;

[SaveDuringPlay]
[AddComponentMenu("")]
public class SensitivityExtension : CinemachineExtension
{
    [SerializeField] private float baseAccel;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float adsAccel;
    [SerializeField] private float adsSpeed;

    private float acceleration;
    private float maxSpeed;

    [Tooltip("When to apply the adjustment")]
    public CinemachineCore.Stage ApplyAfter = CinemachineCore.Stage.Noise;

    private void Start()
    {
        acceleration = baseAccel;
        maxSpeed = baseSpeed;
    }

    public void OnToggleADS(bool isADS)
    {
        if (isADS)
        {
            acceleration = adsAccel;
            maxSpeed = adsSpeed;
        }
        else
        {
            acceleration = baseAccel;
            maxSpeed = baseSpeed;
        }
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        CinemachinePOV cinemachinePOV = vcam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();

        cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = maxSpeed;
        cinemachinePOV.m_HorizontalAxis.m_AccelTime = acceleration;
        cinemachinePOV.m_HorizontalAxis.m_DecelTime = acceleration;

        cinemachinePOV.m_VerticalAxis.m_MaxSpeed = maxSpeed;
        cinemachinePOV.m_VerticalAxis.m_AccelTime = acceleration;
        cinemachinePOV.m_VerticalAxis.m_DecelTime = acceleration;
    }
}

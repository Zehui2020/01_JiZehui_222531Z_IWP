using UnityEngine;
using Cinemachine;

[SaveDuringPlay]
[AddComponentMenu("")]
public class RecoilExtension : CinemachineExtension
{
    [SerializeField] private float snapiness;
    [SerializeField] private float returnSpeed;

    [SerializeField] private Vector3 currentRotation;
    [SerializeField] private Vector3 targetRotation;

    [Tooltip("When to apply the adjustment")]
    public CinemachineCore.Stage ApplyAfter = CinemachineCore.Stage.Noise;

    Quaternion m_RotationCorrection = Quaternion.identity;

    public void ApplyRecoil(float recoilX, float recoilY)
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), 0);
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == ApplyAfter)
        {
            targetRotation = Vector3.Slerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapiness * Time.deltaTime);
            m_RotationCorrection = Quaternion.Euler(currentRotation);

            state.OrientationCorrection = state.OrientationCorrection * m_RotationCorrection;
        }
    }
}
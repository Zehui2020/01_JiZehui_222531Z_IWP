using Cinemachine;
using UnityEngine;

[SaveDuringPlay]
[AddComponentMenu("Cinemachine/Custom/DeathExtension")]
public class DeathExtension : CinemachineExtension
{
    private bool isDead = false;

    [SerializeField] private Transform topDownCam;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (!isDead)
            return;

        if (stage == CinemachineCore.Stage.Aim)
            state.RawOrientation = Quaternion.identity;

        state.RawOrientation = topDownCam.rotation;
        state.RawPosition = topDownCam.position;
    }

    public void OnDie()
    {
        isDead = true;
    }
}
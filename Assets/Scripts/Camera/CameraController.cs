using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera firstPersonCamera;
    public float mouseSensitivity = 2.0f;
    private Vector2 _mousePosition = Vector2.zero;

    [SerializeField] private Transform camFollow;

    public void Initialise()
    {
        firstPersonCamera = Camera.main;
    }

    public void ReadMouseAxisCommand(float MouseX, float MouseY)
    {
        _mousePosition.x += MouseX * mouseSensitivity;
        _mousePosition.y -= MouseY * mouseSensitivity;
    }

    public void UpdateTransform()
    {
        HandleCheck();
        UpdateCamera();
    }

    private void HandleCheck()
    {
        //This is to prevent Camera Flipping
        _mousePosition.y = Mathf.Clamp(_mousePosition.y, -80f, 80f);
    }

    private void UpdateCamera()
    {
        firstPersonCamera.transform.localRotation = Quaternion.Euler(_mousePosition.y, _mousePosition.x, 0);
        firstPersonCamera.transform.position = camFollow.position;
    }
}
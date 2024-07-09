using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    public Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    public Vector3 swayEulerRot;

    public float smooth = 10f;
    public float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    public Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    public Vector3 bobEulerRotation;

    Vector2 walkInput;
    Vector2 lookInput;

    Vector3 initialPosition;
    Quaternion initialRotation;

    void Start()
    {
        // Store initial position and rotation
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    public void UpdateWeaponSway(float horizontal, float vertical, float mouseX, float mouseY, bool isGrounded)
    {
        walkInput.x = horizontal;
        walkInput.y = vertical;
        walkInput = walkInput.normalized;

        lookInput.x = mouseX;
        lookInput.y = mouseY;

        Sway();
        SwayRotation();
        BobOffset(isGrounded, horizontal, vertical);
        BobRotation();
    }

    void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = initialPosition + invertLook;
    }

    public void SetBobExaggeration(float newBob)
    {
        bobExaggeration = newBob;
    }

    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = initialRotation.eulerAngles + new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void BobOffset(bool isGrounded, float horizontal, float vertical)
    {
        float movementMagnitude = new Vector2(horizontal, vertical).magnitude;
        if (isGrounded && movementMagnitude > 0)
        {
            speedCurve += Time.deltaTime * movementMagnitude * bobExaggeration;

            bobPosition.x = (curveCos * bobLimit.x) - (walkInput.x * travelLimit.x);
            bobPosition.y = (curveSin * bobLimit.y);
            bobPosition.z = -(walkInput.y * travelLimit.z);
        }
        else
            bobPosition = Vector3.zero;
    }

    void BobRotation()
    {
        if (walkInput != Vector2.zero)
        {
            bobEulerRotation.x = multiplier.x * Mathf.Sin(2 * speedCurve);
            bobEulerRotation.y = multiplier.y * curveCos;
            bobEulerRotation.z = -multiplier.z * curveCos * walkInput.x;
        }
        else
        {
            bobEulerRotation.x = multiplier.x * Mathf.Sin(2 * speedCurve) / 2;
            bobEulerRotation.y = 0;
            bobEulerRotation.z = 0;
        }
    }
}

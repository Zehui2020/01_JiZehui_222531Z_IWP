using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DesignPatterns.ObjectPool;

public class DamagePopup : PooledObject
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float driftDistance;

    private Animator animator;

    public enum ColorType
    {
        WHITE,
        YELLOW,
        RED
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetupPopup(int damage, Vector3 position, ColorType colorType)
    {
        transform.position = position;
        transform.forward = Camera.main.transform.forward;
        textMesh.SetText(damage.ToString());

        switch (colorType)
        {
            case ColorType.WHITE:
                textMesh.color = Color.white;
                break;
            case ColorType.YELLOW:
                textMesh.color = Color.yellow;
                break;
            case ColorType.RED:
                textMesh.color = Color.red;
                break;
        }

        Deactivate();
    }

    public void Deactivate()
    {
        StartCoroutine(DeactivateRoutine());
    }

    private IEnumerator DeactivateRoutine()
    {
        float timer = 0;
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        Vector3 initialPosition = transform.position;
        Vector3 randomDirection = Random.onUnitSphere;
        if (randomDirection.y < 0)
            randomDirection.y *= -1;

        randomDirection.Normalize();
        Vector3 targetPosition = initialPosition + randomDirection * driftDistance;

        while (timer < animationLength)
        {
            float t = timer / animationLength;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            timer += Time.deltaTime;

            yield return null;
        }

        Release();
        gameObject.SetActive(false);
    }
}
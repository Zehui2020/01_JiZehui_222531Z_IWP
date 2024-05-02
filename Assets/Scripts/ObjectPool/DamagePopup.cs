using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DesignPatterns.ObjectPool;

public class DamagePopup : PooledObject
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetupPopup(int damage)
    {
        textMesh.SetText(damage.ToString());
    }

    public void Deactivate()
    {
        StartCoroutine(DeactivateRoutine());
    }

    private IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Release();
        gameObject.SetActive(false);
    }
}
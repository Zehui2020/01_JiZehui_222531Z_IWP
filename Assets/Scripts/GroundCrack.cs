using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCrack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float damageInterval;
    [SerializeField] private int damage;

    private Coroutine damageRoutine;

    public void SetupGroundCrack(float duration, int newDamage)
    {
        damage = newDamage;
        StartCoroutine(DestroyRoutine(duration));
    }

    private IEnumerator DestroyRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.SetTrigger("Destroy");
    }

    public void DestroyCrack()
    {
        Destroy(gameObject);
    }

    private IEnumerator DamageRoutine()
    {
        PlayerController.Instance.TakeDamage(damage);
        yield return new WaitForSeconds(damageInterval);
        damageRoutine = null;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && damageRoutine == null)
            damageRoutine = StartCoroutine(DamageRoutine());
            
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player") && damageRoutine == null)
            damageRoutine = StartCoroutine(DamageRoutine());
    }
}
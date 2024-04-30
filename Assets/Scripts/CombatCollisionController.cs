using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCollisionController : MonoBehaviour
{
    [SerializeField] private SphereCollider[] colliders;
    [SerializeField] LayerMask targetLayer;

    private Coroutine checkDamageRoutine;

    private void Start()
    {
        foreach (var collider in colliders)
            collider.enabled = false;
    }

    public void EnableCollider(int col)
    {
        colliders[col].enabled = true;
    }

    public void DisableCollider(int col)
    {
        colliders[col].enabled = false;
    }

    public void StartDamageCheck(int damage)
    {
        checkDamageRoutine = StartCoroutine(DamageTarget(damage));
    }

    public void StopDamageCheck()
    {
        if (checkDamageRoutine == null)
            return;

        StopCoroutine(checkDamageRoutine);
        checkDamageRoutine = null;
    }

    private IEnumerator DamageTarget(int damage)
    {
        while (true)
        {
            foreach (var collider in colliders)
            {
                if (!collider.enabled)
                    continue;

                int maxColliders = 10;
                Collider[] hitColliders = new Collider[maxColliders];
                int numColliders = Physics.OverlapSphereNonAlloc(collider.transform.position, collider.radius, hitColliders, targetLayer.value);

                for (int i = 0; i < numColliders; i++)
                {
                    GameObject hitGameObject = hitColliders[i].gameObject;
                    Stats stats = hitGameObject.GetComponent<Stats>();

                    if (stats == null)
                        continue;

                    stats.DealDamage(damage);
                    StopDamageCheck();
                    yield break;
                }
            }

            yield return null;
        }
    }
}
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
                    Collider hitCollider = hitColliders[i];
                    Vector3 closestPoint = hitCollider.ClosestPointOnBounds(collider.transform.position);

                    GameObject hitGameObject = hitCollider.gameObject;

                    EnemyStats enemyStats = GetTopmostParent(hitGameObject.transform).GetComponent<EnemyStats>();
                    PlayerStats playerStats = GetTopmostParent(hitGameObject.transform).GetComponent<PlayerStats>();

                    if (enemyStats != null)
                    {
                        enemyStats.TakeDamage(damage, closestPoint, DamagePopup.ColorType.WHITE);
                        StopDamageCheck();
                        yield break;
                    }
                    else if (playerStats != null)
                    {
                        playerStats.TakeDamage(damage);
                        StopDamageCheck();
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    private Transform GetTopmostParent(Transform child)
    {
        Transform parent = child.parent;

        while (parent != null)
        {
            child = parent;
            parent = child.parent;
        }

        return child;
    }
}
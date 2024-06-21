using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> ragdollRBs = new List<Rigidbody>();
    [SerializeField] private List<Collider> ragdollCols = new List<Collider>();

    public void ActivateRagdoll(Vector3 pushbackDirection, float pushbackForce)
    {
        foreach (Rigidbody rb in ragdollRBs)
        {
            rb.isKinematic = false;
            if (!pushbackDirection.Equals(Vector3.zero))
                rb.AddForce(pushbackDirection * pushbackForce, ForceMode.Impulse);
            else
                rb.AddForce(-transform.forward * pushbackForce / 5f, ForceMode.Impulse);
        }

        foreach (Collider col in ragdollCols)
            col.enabled = true;
    }

    public void ExplosionRagdoll(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        foreach (Rigidbody rb in ragdollRBs)
            rb.isKinematic = false;

        foreach (Collider col in ragdollCols)
            col.enabled = true;

        foreach (Rigidbody rb in ragdollRBs)
            rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
    }

    public void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in ragdollRBs)
            rb.isKinematic = true;

        foreach (Collider col in ragdollCols)
            col.enabled = false;
    }
}
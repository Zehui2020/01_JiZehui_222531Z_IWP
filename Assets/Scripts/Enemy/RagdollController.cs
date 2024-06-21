using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> ragdollRBs = new List<Rigidbody>();
    [SerializeField] private List<Collider> ragdollCols = new List<Collider>();

    public void ActivateRagdoll()
    {
        foreach (Rigidbody rb in ragdollRBs)
            rb.isKinematic = false;

        foreach (Collider col in ragdollCols)
            col.enabled = true;
    }

    public void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in ragdollRBs)
            rb.isKinematic = true;

        foreach (Collider col in ragdollCols)
            col.enabled = false;
    }
}
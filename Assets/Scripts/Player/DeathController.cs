using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathController : MonoBehaviour
{
    [SerializeField] private RagdollController ragdollController;
    [SerializeField] private LayerMask shownLayer;

    public void InitDeathController()
    {
        ragdollController.DeactivateRagdoll();
    }

    public void Die()
    {
        SetLayerRecursively(ragdollController.gameObject, shownLayer.value);
        ragdollController.ActivateRagdoll(transform.forward, 0f);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}

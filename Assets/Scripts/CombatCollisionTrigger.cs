using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCollisionTrigger : MonoBehaviour
{
    [SerializeField] private Collider col;

    public event System.Action<Collider> TriggerEvent;

    public void SetCollider(bool enable)
    {
        col.enabled = enable;
    }

    private void OnTriggerEnter(Collider col)
    {
        TriggerEvent?.Invoke(col);
    }
}
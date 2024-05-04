using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class ItemPickup : PooledObject
{
    [SerializeField] private Item item;

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Player"))
            return;

        PlayerController.Instance.AddItem(item);

        Release();
        gameObject.SetActive(false);
    }
}
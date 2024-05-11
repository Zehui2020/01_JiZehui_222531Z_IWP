using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class ItemPickup : PooledObject
{
    [SerializeField] private Item item;
    [SerializeField] private float rotationSpeed;

    public override void InitPrefab()
    {
        objectName = item.title;
    }

    public void PickupItem()
    {
        PlayerController.Instance.AddItem(item);

        Release();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
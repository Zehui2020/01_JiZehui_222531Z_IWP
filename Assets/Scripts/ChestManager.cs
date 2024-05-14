using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPatterns.ObjectPool;

public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance;

    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private Transform[] chestSpawnPoints;

    [SerializeField] List<PooledObject> chests = new List<PooledObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetupChests();
    }

    public void SetupChests()
    {
        

        int spawnCount = chestSpawnPoints.Length;

        int normalChestCount = Mathf.CeilToInt(spawnCount * 0.6f);
        int largeChestCount = Mathf.CeilToInt(spawnCount * 0.05f);
        int weaponChestCount = Mathf.CeilToInt(spawnCount * 0.05f);
        int specialChestCount = Mathf.CeilToInt((spawnCount - normalChestCount - largeChestCount) / 3f);

        for (int i = 0; i < normalChestCount; i++)
            chests.Add(ObjectPool.Instance.GetPooledObject("NormalChest", true));

        for (int i = 0; i < largeChestCount; i++)
            chests.Add(ObjectPool.Instance.GetPooledObject("LargeChest", true));

        for (int i = 0; i < weaponChestCount; i++)
            chests.Add(ObjectPool.Instance.GetPooledObject("WeaponChest", true));

        for (int i = 0; i < specialChestCount; i++)
            chests.Add(ObjectPool.Instance.GetPooledObject("HealingChest", true));

        for (int i = 0; i < specialChestCount; i++)
            chests.Add(ObjectPool.Instance.GetPooledObject("DamageChest", true));

        for (int i = 0; i < specialChestCount; i++)
            chests.Add(ObjectPool.Instance.GetPooledObject("UtilityChest", true));

        Shuffle(chests);

        foreach (PooledObject chest in chests)
            chest.gameObject.SetActive(false);

        for (int i = 0; i < chestSpawnPoints.Length; i++)
        {
            chests[i].transform.position = chestSpawnPoints[i].position;
            chests[i].gameObject.SetActive(true);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public ItemPickup GetItemPickup(Item.ItemCatagory? itemCatagory, float commonRarity, float uncommonRarity, float legenaryRarity)
    {
        int randNum = Random.Range(0, 100);
        ItemPickup itemPickup = null;

        if (randNum < commonRarity)
            itemPickup = GetRandomItemFromCatagory(itemCatagory, Item.Rarity.Common);
        else if (randNum < commonRarity + uncommonRarity)
            itemPickup = GetRandomItemFromCatagory(itemCatagory, Item.Rarity.Uncommon);
        else if (randNum < uncommonRarity + legenaryRarity)
            itemPickup = GetRandomItemFromCatagory(itemCatagory, Item.Rarity.Legendary);

        return itemPickup;
    }

    private ItemPickup GetRandomItemFromCatagory(Item.ItemCatagory? itemCatagory, Item.Rarity rarity)
    {
        List<Item> itemPool = new List<Item>();

        if (itemCatagory != null)
            itemPool.AddRange(SortItemByRarity(rarity, SortItemByCatagory(itemCatagory)));
        else
            itemPool.AddRange(SortItemByRarity(rarity, items));

        if (itemPool.Count == 0)
            return null;

        int randNum = Random.Range(0, itemPool.Count);

        return ObjectPool.Instance.GetPooledObject(itemPool[randNum].title, true).GetComponent<ItemPickup>();
    }

    private List<Item> SortItemByCatagory(Item.ItemCatagory? itemCatagory)
    {
        List<Item> catagorizedItems = new List<Item>();

        foreach (Item item in items)
        {
            if (item.itemCatagory == itemCatagory)
                catagorizedItems.Add(item);
        }

        return catagorizedItems;
    }

    private List<Item> SortItemByRarity(Item.Rarity itemRarity, List<Item> itemList)
    {
        List<Item> filteredItems = new List<Item>();

        foreach (Item item in itemList)
        {
            if (item.itemRarity == itemRarity)
                filteredItems.Add(item);
        }

        return filteredItems;
    }
}
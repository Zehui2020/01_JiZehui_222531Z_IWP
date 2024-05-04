using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : ScriptableObject
{
    public ItemStats itemStats;

    public enum Rarity { Common, Rare, Legendary, Special };
    public Rarity itemRarity;

    public enum ItemType { HDHUD };
    public ItemType itemType;

    public Sprite spriteIcon;
    [TextArea(3, 10)]
    public string title;
    [TextArea(3, 10)]
    public string description;
    public int itemStack;

    public virtual void Initialize() { }

    public virtual void IncrementStack() { itemStack++; }

    public virtual void DecrementStack() { itemStack--; }
}
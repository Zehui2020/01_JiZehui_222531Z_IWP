using UnityEngine;

[CreateAssetMenu(menuName = "Items/BlackCard")]
public class BlackCard : Item
{
    [SerializeField] private int freeChestCount;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.freeChests += freeChestCount;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.freeChests += freeChestCount;
    }
}
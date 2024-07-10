using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/JackInTheBox")]
public class JackInTheBox : Item
{
    [SerializeField] private float baseStunRadius;
    [SerializeField] private float stackStunRadius;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.jitbStunRadius += baseStunRadius;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.jitbStunRadius += stackStunRadius;
    }
}
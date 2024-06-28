using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Companion/CompanionMessenger")]
public class CompanionMessenger : ScriptableObject
{
    public CompanionMessage[] introMessages;
}
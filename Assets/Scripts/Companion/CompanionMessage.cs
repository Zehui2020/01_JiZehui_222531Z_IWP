using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Companion/CompanionMessage")]
public class CompanionMessage : ScriptableObject
{
    [TextArea(3, 10)]
    public string message;
    public float waitDuration;
    public float entryDelay;
    public float timeBtwChars = 0.05f;
    public bool hideAfterMessage;
}
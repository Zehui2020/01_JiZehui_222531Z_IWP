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
    public float textDelay = 0.3f;
    public float timeBtwChars = 0.05f;
    public bool isPartOfList;
    public bool hideAfterMessage;
    public bool overridePreviousMessage;
}
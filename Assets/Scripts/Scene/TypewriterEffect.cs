using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] string leadingChar = "";

    [TextArea(3, 10)]
    [SerializeField] string message = "";

    [SerializeField] float timeBtwChars = 0.5f;
    [SerializeField] private TextMeshProUGUI companionText;

    public UnityEvent OnFinishTyping;

    private void OnEnable()
    {
        StartCoroutine(TypeWriterTMP(message));
    }

    private IEnumerator TypeWriterTMP(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] == '<')
            {
                string richTextTag = GetCompleteRichTextTag(ref i, message);
                companionText.text += richTextTag;
            }
            else
            {
                companionText.text += message[i];
            }

            if (leadingChar != "")
            {
                companionText.text += leadingChar;
                yield return new WaitForSeconds(timeBtwChars);
                companionText.text = companionText.text.Substring(0, companionText.text.Length - leadingChar.Length);
            }
            else
            {
                yield return new WaitForSeconds(timeBtwChars);
            }
        }

        OnFinishTyping?.Invoke();
    }

    private string GetCompleteRichTextTag(ref int index, string message)
    {
        string completeTag = string.Empty;

        while (index < message.Length)
        {
            completeTag += message[index];

            if (message[index] == '>')
                return completeTag;

            index++;
        }

        return string.Empty;
    }
}
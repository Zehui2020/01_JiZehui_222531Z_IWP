using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CompanionManager : MonoBehaviour
{
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;

	[SerializeField] private Animator animator;
	[SerializeField] private TextMeshProUGUI companionText;

	private Coroutine MessageRoutine;
    private Coroutine SkipRoutine;

    public event System.Action OnMessageFinish;
	public static CompanionManager Instance;

    private void Awake()
    {
		Instance = this;
    }

    public void ShowMessages(CompanionMessage[] companionMessages)
    {
        StartCoroutine(ShowMessagesRoutine(companionMessages));
    }

    private IEnumerator ShowMessagesRoutine(CompanionMessage[] companionMessages)
    {
        for (int i = 0; i < companionMessages.Length; i++)
        {
            if (i < companionMessages.Length - 1)
                ShowMessage(companionMessages[i], true);
            else
                ShowMessage(companionMessages[i], false);

            while (MessageRoutine != null)
                yield return null;
        }
    }

    public void ShowMessage(CompanionMessage companionMessage, bool isShowingList = false)
    {
		if (MessageRoutine != null)
			return;

		MessageRoutine = StartCoroutine(TypeWriterTMP(companionMessage, isShowingList));
    }

    private IEnumerator TypeWriterTMP(CompanionMessage companionMessage, bool isDisplayingList)
    {
        companionText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(companionMessage.entryDelay);

        animator.SetTrigger("show");
        foreach (char c in companionMessage.message)
        {
            if (companionText.text.Length > 0)
                companionText.text = companionText.text.Substring(0, companionText.text.Length - leadingChar.Length);

            companionText.text += c;
            companionText.text += leadingChar;
            yield return new WaitForSeconds(companionMessage.timeBtwChars);
        }

        if (leadingChar != "")
            companionText.text = companionText.text.Substring(0, companionText.text.Length - leadingChar.Length);

        yield return new WaitForSeconds(companionMessage.waitDuration);

        if (companionMessage.hideAfterMessage)
            animator.SetTrigger("hide");

        MessageRoutine = null;

        if (!isDisplayingList)
            OnMessageFinish?.Invoke();
    }

    public void SkipMessage()
    {
        if (SkipRoutine == null)
            SkipRoutine = StartCoroutine(DoSkipRoutine());
    }

    private IEnumerator DoSkipRoutine()
    {
        if (MessageRoutine == null)
            yield break;

        StopCoroutine(MessageRoutine);
        MessageRoutine = null;
        animator.SetTrigger("hide");

        yield return new WaitForSeconds(1f);

        SkipRoutine = null;
    }
}
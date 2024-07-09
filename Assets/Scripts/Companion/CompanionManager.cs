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

    public CompanionMessenger companionMessenger;
    public event System.Action OnMessageFinish;
	public static CompanionManager Instance;

    private void Awake()
    {
		Instance = this;
    }

    public void ShowRandomMessage(CompanionMessage[] messages)
    {
        int randNum = Random.Range(0, messages.Length);
        ShowMessage(messages[randNum]);
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
                ShowMessage(companionMessages[i]);
            else
                ShowMessage(companionMessages[i]);

            while (MessageRoutine != null)
                yield return null;
        }
    }

    public void ShowMessage(CompanionMessage companionMessage)
    {
		if (MessageRoutine != null)
        {
            if (!companionMessage.overridePreviousMessage)
                return;
            else
            {
                StopCoroutine(MessageRoutine);
                MessageRoutine = null;

                if (!companionMessage.isPartOfList)
                {
                    OnMessageFinish?.Invoke();
                    OnMessageFinish = null;
                }
            }
        }

		MessageRoutine = StartCoroutine(TypeWriterTMP(companionMessage));
    }

    private IEnumerator TypeWriterTMP(CompanionMessage companionMessage)
    {
        companionText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(companionMessage.entryDelay);

        animator.SetTrigger("show");

        yield return new WaitForSeconds(companionMessage.textDelay);

        AudioManager.Instance.Play(Sound.SoundName.MorseCode);

        for (int i = 0; i < companionMessage.message.Length; i++)
        {
            if (companionMessage.message[i] == '<')
            {
                string richTextTag = GetCompleteRichTextTag(ref i, companionMessage.message);
                companionText.text += richTextTag;
            }
            else
            {
                companionText.text += companionMessage.message[i];
            }

            if (leadingChar != "")
            {
                companionText.text += leadingChar;
                yield return new WaitForSeconds(companionMessage.timeBtwChars);
                companionText.text = companionText.text.Substring(0, companionText.text.Length - leadingChar.Length);
            }
            else
            {
                yield return new WaitForSeconds(companionMessage.timeBtwChars);
            }
        }

        AudioManager.Instance.FadeSound(false, Sound.SoundName.MorseCode, 0.2f, 0);

        yield return new WaitForSeconds(companionMessage.waitDuration);

        if (companionMessage.hideAfterMessage)
        {
            animator.ResetTrigger("show");
            animator.SetTrigger("hide");
        }

        MessageRoutine = null;

        if (!companionMessage.isPartOfList)
        {
            OnMessageFinish?.Invoke();
            OnMessageFinish = null;
        }
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

        animator.ResetTrigger("show");
        animator.SetTrigger("hide");

        yield return new WaitForSeconds(1f);

        SkipRoutine = null;
    }

    public void ShowWeaponPickupMessage(WeaponData.Weapon weaponType)
    {
        foreach (WeaponPickupMessage message in companionMessenger.weaponPickupMessages)
        {
            if (message.weaponType == weaponType)
            {
                ShowMessage(message);
                return;
            }
        }
    }

    public void ShowVehiclePartFoundMessage(VehiclePart.VehiclePartType vehiclePartType)
    {
        OnMessageFinish += () => { PlayerController.Instance.SetWaveObjectiveToSpawnVehiclePart(3); };

        foreach (VehiclePartFoundMessage message in companionMessenger.vehiclePartFoundMessages)
        {
            if (message.vehiclePartType == vehiclePartType)
            {
                ShowMessage(message);
                return;
            }
        }
    }

    public void ShowVehiclePartPickupMessage(VehiclePart vehiclePart)
    {
        int randNum = Random.Range(0, companionMessenger.vehiclePartPickupMessages.Length);
        ShowMessage(companionMessenger.vehiclePartPickupMessages[randNum]);

        VehicleToTruckObjective objective = new VehicleToTruckObjective(Objective.ObjectiveType.Normal, "Bring the " + vehiclePart.vehiclePartName + " back to the truck", vehiclePart.vehiclePartType);
        ObjectiveManager.Instance.AddObjective(objective);
    }
}
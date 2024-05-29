using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance;
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SetConsole()
    {
        if (!gameObject.activeInHierarchy)
        {
            Time.timeScale = 0;
            gameObject.SetActive(true);
            inputField.Select();
        }
        else
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);

        }
    }

    public void OnInputCommand()
    {
        if (!gameObject.activeInHierarchy)
            return;

        string[] words = inputField.text.Split(' ');
        string command = words[0];

        if (command.Equals("/give"))
        {
            if (words[1].Equals("item"))
                GiveItem(words[2], words[3]);

            else if (words[1].Equals("weapon"))
                PlayerController.Instance.ReplaceWeapon((WeaponData.Weapon)System.Enum.Parse(typeof(WeaponData.Weapon), words[2]));

            else if (words[1].Equals("points"))
                PlayerController.Instance.AddPoints(int.Parse(words[2]));
        }
    }

    private void GiveItem(string itemName, string amount)
    {
        foreach (Item item in ChestManager.Instance.items)
        {
            if (!item.itemType.ToString().Equals(itemName))
                continue;

            for (int i = 0; i < int.Parse(amount); i++)
                PlayerController.Instance.AddItem(item);
        }
    }
}
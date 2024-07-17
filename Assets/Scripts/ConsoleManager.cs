using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private Transform indoors;
    [SerializeField] private Transform outdoors;

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
            {
                if (words[2].Equals("all"))
                    GiveAllItems();
                else
                    GiveItem(words[2], words[3]);
            }

            else if (words[1].Equals("weapon"))
                PlayerController.Instance.ReplaceWeapon((WeaponData.Weapon)System.Enum.Parse(typeof(WeaponData.Weapon), words[2]));

            else if (words[1].Equals("points"))
                PlayerController.Instance.AddPoints(int.Parse(words[2]));
        }
        else if (command.Equals("/wave"))
            EnemySpawner.Instance.SetWave(int.Parse(words[1]));
        else if (command.Equals("/refill"))
        {
            PlayerController.Instance.RefillAmmoClip();
            PlayerController.Instance.RestockCurrentWeapon();
        }
        else if (command.Equals("/spawn"))
        {
            EnemySpawner.Instance.SpawnEnemyAtPosition(
                (Enemy.EnemyType)System.Enum.Parse(typeof(Enemy.EnemyType), words[1]),
                PlayerController.Instance.transform.position);
        }
        else if (command.Equals("/win"))
        {
            LevelManager.Instance.LoadScene("WinScreen");
        }
        else if (command.Equals("/health"))
        {
            PlayerController.Instance.health = int.Parse(words[1]);
        }
        else if (command.Equals("/tp"))
        {
            if (words[1].Equals("in"))
                PlayerController.Instance.transform.position = indoors.position;
            else
                PlayerController.Instance.transform.position = outdoors.position;
        }

        SetConsole();
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

    private void GiveAllItems()
    {
        foreach (Item item in ChestManager.Instance.items)
            PlayerController.Instance.AddItem(item);
    }
}
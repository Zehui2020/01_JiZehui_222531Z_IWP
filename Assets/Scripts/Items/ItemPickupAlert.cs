using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ItemPickupAlert : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    private List<Item> itemsToDisplay = new List<Item>();

    public void DisplayAlert(Item item)
    {
        itemsToDisplay.Add(item);
        ShowItem();
    }

    private void ShowItem()
    {
        itemIcon.sprite = itemsToDisplay[0].spriteIcon;
        title.text = itemsToDisplay[0].title;
        description.text = itemsToDisplay[0].description;

        animator.SetTrigger("show");
    }

    public void RemoveItem()
    {
        itemsToDisplay.RemoveAt(0);
        if (itemsToDisplay.Count > 0)
            ShowItem();
    }
}
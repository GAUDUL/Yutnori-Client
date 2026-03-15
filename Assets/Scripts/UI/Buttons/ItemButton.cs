using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private string itemId;
    private Action<string> onClick;

    public void Initialize(Item item, Action<string> onClick)
    {

        buttonText.text = item.Name;
        this.itemId = item.Id;
        this.onClick = onClick;

        button.onClick.AddListener(Click);
    }

    private void Click()
    {
        onClick?.Invoke(itemId);
    }
}
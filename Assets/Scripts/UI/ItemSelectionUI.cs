using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform itemPanel;
    [SerializeField] private GameObject itemButtonPrefab;

    private Action<string> onItemSelected;

    public void Show(List<Item> items, Action<string> onItemSelected)
    {
        // 선택 시 실행시킬 Action
        this.onItemSelected = onItemSelected;

        gameObject.SetActive(true);

        foreach (Transform child in itemPanel)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            GameObject obj = Instantiate(itemButtonPrefab, itemPanel);
            var button = obj.GetComponent<ItemButton>();
            button.Initialize(item, OnClickItem);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClickItem(string itemId)
    {
        onItemSelected?.Invoke(itemId);
    }
}
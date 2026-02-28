using UnityEngine;
using UnityEngine.UI;
using System;

public class MergeSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button mergeButton;
    [SerializeField] private Button dontMergeButton;

    private Action<bool> onSelected;

    private void Awake()
    {
        panel.SetActive(false);

        mergeButton.onClick.AddListener(() => Select(true));
        dontMergeButton.onClick.AddListener(() => Select(false));
    }

    public void Show(Action<bool> callback)
    {
        onSelected = callback;
        panel.SetActive(true);
    }

    private void Select(bool merge)
    {
        panel.SetActive(false);
        onSelected?.Invoke(merge);
    }
}
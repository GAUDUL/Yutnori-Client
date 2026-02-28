using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepSelectionUI : MonoBehaviour
{
    private Action<int> onSelectStep;

    [SerializeField] private GameObject stepButtonPrefab;
    [SerializeField] private Transform container;

    private List<GameObject> buttons = new List<GameObject>();

    public void Show(List<int> steps, bool selectable, Action<int> onSelect)
    {
        Clear();

        onSelectStep = selectable ? onSelect : null;

        foreach (var step in steps)
        {
            GameObject obj = Instantiate(stepButtonPrefab, container);
            buttons.Add(obj);

            var button = obj.GetComponent<Button>();
            var text = obj.GetComponentInChildren<TextMeshProUGUI>();

            text.text = step.ToString();

            int capturedStep = step;

            button.interactable = selectable;
            button.onClick.AddListener(() => SelectStep(capturedStep));

        }
    }

    public void SelectStep(int step)
    {
        onSelectStep?.Invoke(step);
    }

    public void Hide()
    {
        Clear();
        onSelectStep = null;
    }

    private void Clear()
    {
        foreach (var b in buttons)
            Destroy(b);

        buttons.Clear();
    }
}
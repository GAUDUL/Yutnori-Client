using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    private string playerId;
    private Action<string> onClick;

    public void Initialize(Player player, Action<string> onClick)
    {
        this.playerId = player.PlayerId;
        this.onClick = onClick;

        text.text = player.DisplayName;

        button.onClick.AddListener(Click);
    }

    private void Click()
    {
        onClick?.Invoke(playerId);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform playerListPanel;
    [SerializeField] private GameObject playerButtonPrefab;

    private Action<string> onPlayerSelected;

    public void Show(List<Player> players, Action<string> onPlayerSelected)
    {
        this.onPlayerSelected = onPlayerSelected;

        gameObject.SetActive(true);

        foreach (Transform child in playerListPanel)
            Destroy(child.gameObject);

        foreach (var player in players)
        {
            var obj = Instantiate(playerButtonPrefab, playerListPanel);
            var button = obj.GetComponent<PlayerButton>();

            button.Initialize(player, OnClickPlayer);
        }
    }

    private void OnClickPlayer(string playerId)
    {
        onPlayerSelected?.Invoke(playerId);
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
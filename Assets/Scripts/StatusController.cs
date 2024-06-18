using UnityEngine;
using TMPro;

namespace Airinterface.TicTacToe
{
    public class StatusController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI statusLabel;
        
        private void OnPlayerJoinedCallback(Player player)
        {
            statusLabel.text = $"player joined: {player.id}";
            // React to player joining
            Debug.Log($"Handling player joined: {player.id} Waiting for other");
        }

        private void HandlePlayerWonCallback(Player player)
        {
            statusLabel.text = $"player won: {player.id}";
            // React to player winning
            Debug.Log($"Handling player won: {player.id}");
        }

        private void onGameOver(string message) {
            statusLabel.text = message;
        }

        private void onLoadingNetwork() {
            statusLabel.text = $"Starting up..... ";
        }

        private void onWaitingOther()
        {
            statusLabel.text = $"You Joined waiting for other..... ";
        }

        private void OnReadyCallback() {
            statusLabel.text = $"Start Playing";
        }
        private void OnPlayerStartPlayingCallback() {
            statusLabel.text = "";
        }

    }
}
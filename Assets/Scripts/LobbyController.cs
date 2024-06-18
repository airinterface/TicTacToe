using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Airinterface.TicTacToe
{
    public class LobbyController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartSinglePlayer()
        {
            PlayerPrefs.SetString("GameMode", "SinglePlayer");
            SceneManager.LoadScene("GameScene");
        }

        public void StartMultiplayer()
        {
            PlayerPrefs.SetString("GameMode", "Multiplayer");
            SceneManager.LoadScene("GameScene");
        }
    }
}
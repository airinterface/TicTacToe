using UnityEngine;

namespace Airinterface.TicTacToe
{
    [System.Serializable]
    public class Config
    {
        public string ApplicationID;
        public string Region;
        public int Port;
        public string LocalServerAddress;
    }


    public class ConfigurationManager : MonoBehaviour
    {

        public static ConfigurationManager Instance { get; private set; }
        public Config Config { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadConfig();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadConfig()
        {
            TextAsset configText = Resources.Load<TextAsset>("config");
            if (configText != null)
            {
                Config = JsonUtility.FromJson<Config>(configText.text);
                Debug.Log("Application ID: " + Config.ApplicationID);
            }
            else
            {
                Debug.LogError("Config file not found!");
            }
        }
    }
}
/**
 * Author: Yuri Fukuda 
 * created: June 16 2024
 **/

using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;

using System.Collections.Generic;
using System;


namespace Airinterface.TicTacToe
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkManager Instance;

        public NetworkRunner runner;

        private string roomName = "TicTacToe";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            } 

            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }
        }

        public void Start()
        {
            Debug.Log($"#TTT NetworkManager instaciated");
            startNetwork();
        }

        async void startNetwork()
        {
            Debug.Log("#TTT: Network Manager Start Network");
            string appId = ConfigurationManager.Instance.Config.ApplicationID;

            // Create the Fusion runner and let it know that we will be providing user input
            runner.ProvideInput = true;
            // Start or join (depends on gamemode) a session with a specific name

            _ = await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                PlayerCount = 2,
                SessionName = "room_" + appId + "_" + System.DateTime.Now.Ticks,
                IsVisible = true,
                IsOpen = true,
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        public void stopNetwork()
        {
            // Stop Fusion, e.g., shutting down the session
            var runner = GetComponent<NetworkRunner>();
            if (runner != null)
            {
                runner.Shutdown();
                Destroy(runner);
            }
        }



        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("#TTT: Connected To the Server");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            GameManager.Instance.playerJoined(player);
            Debug.Log("Player Joined with playerref: " + player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player Left");
        }

        #region unused

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }


        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {

        }
        #endregion
    }

}


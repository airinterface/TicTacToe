/**
 * Author: Yuri Fukuda 
 * created: June 16 2024
 **/

using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
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

        }

        public async void Start()
        {

            await WaitForConfigLoader();
            string appId = ConfigurationManager.Instance.Config.ApplicationID;
            if (string.IsNullOrEmpty(appId))
            {
                Debug.LogError("Application ID is null or empty");
                return;
            }



            Debug.Log("Using Application ID: " + appId);

            Debug.Log($"#TTT NetworkManager instaciated");
            startNetwork();
        }

        async void startNetwork()
        {
            Debug.Log("#TTT: Network Manager Start Network");
            string appId = ConfigurationManager.Instance.Config.ApplicationID;
            string region = ConfigurationManager.Instance.Config.Region;
            string network = ConfigurationManager.Instance.Config.LocalServerAddress;
            int port = ConfigurationManager.Instance.Config.Port;
            Debug.Log("Using Application ID: " + appId);
            Debug.Log("Using Region: " + region);
            Debug.Log("Using Local Server Address: " + network);
            Debug.Log("Using Local Server Port: " + port);

            Fusion.Photon.Realtime.FusionAppSettings appSettings = new Fusion.Photon.Realtime.FusionAppSettings
            {
                AppIdFusion = appId,
                AuthMode = Fusion.Photon.Realtime.AuthModeOption.Auth,
                //FixedRegion = region,
                //Port = 5055,
                //Server = network,
                Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp

            };
            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }

            // Create the Fusion runner and let it know that we will be providing user input

            runner.ProvideInput = true;



            // Start or join (depends on gamemode) a session with a specific name

            var result = await runner.StartGame(new StartGameArgs()
            {
                SessionName = "room_" + appId + "_" + System.DateTime.Now.Ticks,
                GameMode = GameMode.Shared,
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                CustomPhotonAppSettings = appSettings,
                PlayerCount = 2,
                IsVisible = true,
                IsOpen = true,
            });
            if (result.Ok)
            {
                Debug.Log("Game started successfully");
            }
            else
            {
                Debug.LogError("Failed to start game: " + result.ShutdownReason);
            }
        }

        
        private async Task WaitForConfigLoader()
        {
            while (ConfigurationManager.Instance == null || ConfigurationManager.Instance.Config == null)
            {
                await Task.Yield();
            }
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


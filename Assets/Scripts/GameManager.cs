/**
 * Author: Yuri Fukuda 
 * created: June 16 2024
 **/

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Fusion;
using UnityEngine.UI;
using System;
using TMPro;

namespace Airinterface.TicTacToe
{

    public enum Turn
    {
        First,
        Second
    }


    public enum GameType
    {
        Single,
        Multiplayer
    }

    public enum TextType
    {
        X,
        O
    }

    public class GameManager: NetworkBehaviour
    {
        public GameObject NetworkPrefab;
        public static GameManager Instance;
        public GameType gameType = GameType.Multiplayer;

        [Networked]
        private int currentIndex { get; set; } = 0;
        [Networked]
        private int availableCount { get; set; } = 9;

        private CellController[] cells = new CellController[0];
        private TextType?[] textArray = new TextType?[9];
        private bool initialllyClicked = false;        

        private Turn currentTurn = Turn.First;

        private TextType[] order = new TextType[] { TextType.O, TextType.X };
        private Turn[] turns = new Turn[] { Turn.First, Turn.Second };
        private List<Player> players = new List<Player>();



        // Public static property to provide global access to the instance
        public void Awake()
        {

            GameObject board = GameObject.Find("Board");
            initializeCellsFromBoard(board);
            if (Instance == null)
            {
                Instance = this;
            }

            Instantiate(NetworkPrefab);
            

        }

        public void Start()
        {
            bool _isMultiPlayer = PlayerPrefs.GetString("GameMode") == "Multiplayer";

            setGameType(_isMultiPlayer);
            Debug.Log($"#TTT Game Manager instaciated with {(isMultiPlayer() ? "Multiplayer" : "SinglePlayer")} - {PlayerPrefs.GetString("GameMode")}");
            BroadcastMessage("onLoadingNetwork");
        }


        public void mark(TextType textType, int col, int row)
        {
            RPC_marked(textType, col, row);
        }

        [Rpc]
        public void RPC_marked(TextType textType, int col, int row ) {
            if (!initialllyClicked) {
                initialllyClicked = true;
                BroadcastMessage("OnPlayerStartPlayingCallback");
            }
            int index = getIndexFromColRow(col, row);
            if (HasStateAuthority)
            {
                Debug.Log("#TTT Host changing state col: " + col + ", row: " +
                    row + "For TextType " + (textType == TextType.O ? "O" : "X"));
                textArray[getIndexFromColRow(col, row)] = textType;
            }
            cells[index].clickExecute(textType);
            if (HasStateAuthority) {
                availableCount--;
            }
            int result = evaluate(col, row);
            Debug.Log("#TTT Game Evaluated: " + result + " col,row = " + col + "," + row );

            if (result < 2) {
                String resultString = "";
                if (result == -1) {
                    resultString = "It's a tie";
                } else if (! isMultiPlayer() ) {
                    resultString = result == 0 ? "You Won" : "You Lose";
                } else {
                    bool you = NetworkManager.Instance.runner.LocalPlayer.PlayerId == players[result].id ;
                    resultString = you ? "You won " : "You lose";
                }
                Debug.Log("#TTT  result " + result);
                BroadcastMessage("onGameOver", resultString);
            }
            togglePlayer();
            if (!isMultiPlayer() && currentTurn == Turn.Second)
            {
                StartCoroutine(simulateNextMove());
            }
            else {
                Debug.Log("#TTT Not Simulating next player : result " + result  );
            }
        }

        [Rpc]
        public void RPC_playerJoined(int playerId)
        {
            Player player = new(
                playerId == 0 ? Turn.First : Turn.Second,
                playerId,
                playerId == 0 ? TextType.O : TextType.O);
            players.Add(player);
            if (isMultiPlayer())
            {
                BroadcastMessage("OnPlayerJoinedCallback", player);
                if (playerId == 2)
                {
                    onReadyForGame();
                }
            }
            else {
                setupSimulatedPlayer();
                onReadyForGame();
            }
        }

        public void setupSimulatedPlayer() {
            Player player2 = new(Turn.Second, 2, TextType.X);
            players.Add(player2);
        }

        public void playerJoined(PlayerRef playerRef) {
            Debug.Log($"TTT Player Joined {playerRef}");
            RPC_playerJoined(playerRef.PlayerId);
        }

        public bool isMultiPlayer() {

            return this.gameType == GameType.Multiplayer;
        }

        public void setGameType( bool isMultiPlayer ) {
            this.gameType = isMultiPlayer? GameType.Multiplayer : GameType.Single;
        }

        public void onReadyForGame()
        {
            this.currentTurn = Turn.First;
            this.enableAllCells();
            BroadcastMessage("OnReadyCallback");
        }

        public Player getPlayerFromPlayerID(int id) {
            Debug.Log("#TTT " + players + " index : " + id);
            return players[id-1];
        }

        public void enableAllCells()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].enable();
            }
        }

        public void disableAllCells()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].disable();
            }
        }

        private void initializeCellsFromBoard(GameObject parent)
        {
            int childCount = parent.transform.childCount;
            cells = new CellController[childCount];

            for (int i = 0; i < childCount; i++)
            {

                GameObject child = parent.transform.GetChild(i).gameObject;
                CellController controller  = child.GetComponent<CellController>();
                if (controller != null) {
                    cells[getIndexFromColRow(controller.column, controller.row)] = controller;
                }
            }
            Debug.Log($"#TTT cell registered :{cells.Length}");


        }

        /*
         * 2: Game continues
         * 1: Player2 won
         * 0: Player1 won
         * -1: tie
         */
        private int evaluate( int col, int row ) {
            if ( availableCount > 7) return 2;
            int nextMove = nextAvailable();
            // ---------------- horizontal check
            TextType? winner = null;
            if (textArray[ getIndexFromColRow(0, row) ] == textArray[ getIndexFromColRow(1, row ) ] &&
                textArray[getIndexFromColRow(1, row)] == textArray[getIndexFromColRow(2, row)]) {
                winner = textArray[getIndexFromColRow(col, row)];
            } 
            // --------------- vertical check
            else if (textArray[getIndexFromColRow(col, 0)] == textArray[getIndexFromColRow(col, 1)] &&
               textArray[getIndexFromColRow(col, 1)] == textArray[getIndexFromColRow(col, 2)])
            {
                winner = textArray[getIndexFromColRow(col, row)];
            }
            // ----- cross check
            else if ( textArray[getIndexFromColRow(0, 0)] == textArray[getIndexFromColRow(1, 1)] &&
               textArray[getIndexFromColRow(1, 1)] == textArray[getIndexFromColRow(2, 2)])
            {
                winner = textArray[getIndexFromColRow(1, 1)];
            } else if (textArray[getIndexFromColRow(2, 0)] == textArray[getIndexFromColRow(1, 1)] &&
              textArray[getIndexFromColRow(1, 1)] == textArray[getIndexFromColRow(0, 2)])
            {
                winner = textArray[getIndexFromColRow(1, 1)];
            }

            if (winner != null)
            {
                return winner == players[0].textType ? 0 : 1;
            }
            else if (availableCount == 0)
            {
                return -1;
            }
            return 2;

        }



        private IEnumerator simulateNextMove()
        {
            Debug.Log("#TTT Simulating next Move1");

            if (availableCount != 0)
            {
                int nextMove = nextAvailable();
                if (nextMove >= 0)
                {
                    yield return new WaitForEndOfFrame();
                    int row = nextMove / 3;
                    int colum = nextMove % 3;
                    Debug.Log("#TTT Simulating next Move2");
                    mark(getCurrentPlayer().textType, colum, row);
                }
            }

        }


        /* if  -1 if there is no move */
        private int nextAvailable() {
            for (int i = 0; i < textArray.Length; i++)
            {
                if (!textArray[i].HasValue)
                {
                    return i;
                }
            }
            return -1; // Return -1 if no null values are found
        }


        private int getIndexFromColRow(int col, int row) {
            return (row * 3 + col);
        }


        private void togglePlayer() {
            currentIndex = (currentIndex + 1) % 2;
            Debug.Log("#TTT Next Index " + currentIndex);
            currentTurn = turns[currentIndex];
        }

        private Player getCurrentPlayer() {
            return players[currentIndex];
        }

        public bool click( CellController cell, PlayerRef playerRef)
        {
            Debug.Log($"#TTT Game Manager: click :  ${cell.column}, ${cell.row}");
            if ( canClick( playerRef)) {
                Player player = getPlayerFromPlayerID(playerRef.PlayerId);
                mark(player.textType, cell.column, cell.row);
                return true;
            }
            return false;

        }


        public bool canClick( PlayerRef player ) {
            if (gameType == GameType.Single) { 
                if( currentTurn == Turn.First)
                {
                    return true;
                }
            } else {
                if ((NetworkManager.Instance.runner.LocalPlayer.PlayerId == 0 &&
                     currentTurn == Turn.First) ||
                   (NetworkManager.Instance.runner.LocalPlayer.PlayerId == 1 &&
                     currentTurn == Turn.Second)) {
                    return true;
                }
            }
            return false;

        }

    }

    public class Player
    {
        public Turn turn { get; set; }
        public int  id { get; set; }
        public TextType textType { get; set; }

        public Player(Turn turn, int id, TextType textType)
        {
            this.turn = turn;
            this.id = id;
            this.textType = textType;
        }
    }
}
# TicTacToe

## Goal

This is a Unity game that demostrate peer to peer multiplayer mode.
Player has P2P Network where user can choose Single Player or Second Player.



Flow will be

```
    A[Lobby (Mode Choice)] --> B[P2P Initialization]
    A --> C[Game Starts]
    C --> D[Evaluate Move]
    D --> E[Simulate If Necessary]
    D --> F Wait for the next Input
    E --> C
    F --> C
    D --> B [Game Over]
    
```

# Technical Detail

Network Manager : Wraps Fusion Manager
Game Manager: Handle logic
State Controller: Handles State UI display
Cell Controller: Handles Cell State and display
Next Move Evaluation: Game Manager handles manually


# Running the game

In the file Assets/Resources/config.json
Add [Fusion](https://dashboard.photonengine.com/) pick up your Application ID
and place it there. 

```
{
  "ApplicationID": <Application Id Here> 
}

```

# TODO

1. simulation could be wiser using minimax algorithm.
2. At the GameOver, replay button should bring it back to A->C
3. Multiplayer flow test. 

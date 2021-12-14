using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    MAIN_MENU,
    PLAYING,
    PAUSED,
    GAME_OVER
}

// TODO: dont care about this for now
public enum PlayerTurnState
{
    START_TURN,
    IDLE,
    AIMING, // also includes charging up a throw
    THROWING, 
    END_TURN
}

public enum CurrentPlayer
{
    PLAYER_1 = 1,
    PLAYER_2 = 2
}

public enum WinCondition {HitFloor, LeaveBoundary, Both}


public class GameManager : MonoBehaviour
{
    // basically a singleton - only 1, accessible anywhere, instance can be retrieved anywhere
    public static GameManager Instance;

    // States
    [SerializeField] private GameState currentGameState;
    [SerializeField] private CurrentPlayer currentPlayer;
    [SerializeField] private WinCondition winCondition;

    public static event Action<GameState> OnGameStateChanged;

    // 
    [SerializeField] private GameObject gameStateCube;
    private Renderer cubeRenderer;
    

    //private float timer;
    //[SerializeField] private float changeStateTime = 1;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // set to main menu
        gameStateCube = GameObject.Find("GameStateCube");
        if (gameStateCube != null)
        {
            cubeRenderer = gameStateCube.GetComponent<Renderer>();
        }

        SetCurrentGameState(GameState.MAIN_MENU);
        currentPlayer = CurrentPlayer.PLAYER_1;
    
        //timer = changeStateTime;
    }

    private void Update()
    {
        // if (timer > 0)
        // {
        //     timer -= Time.deltaTime;
        // } 
        // else if (timer <= 0)
        // {
        //     switch(currentGameState)
        //     {
        //         case GameState.MAIN_MENU:
        //             SetCurrentGameState(GameState.PLAYER_1);
        //             break;
        //         case GameState.PLAYER_1:
        //             SetCurrentGameState(GameState.PLAYER_2);
        //             break;
        //         case GameState.PLAYER_2:
        //             SetCurrentGameState(GameState.PLAYER_1);
        //             break;
        //     }
        //     timer = changeStateTime;
        // }
        
    }
    private void UpdateGameState()
    {
        if (cubeRenderer != null)
        {
            switch(currentGameState)
            {
                case GameState.MAIN_MENU:
                    cubeRenderer.material.color = Color.white;
                    break;
                case GameState.PLAYING:
                    //Call SetColor using the shader property name "_Color" and setting the color to red
                    if(currentPlayer == CurrentPlayer.PLAYER_1)
                        cubeRenderer.material.color = Color.yellow;
                    else
                        cubeRenderer.material.color = Color.blue;
                    break;
                case GameState.GAME_OVER:
                    cubeRenderer.material.color = Color.red;
                    break;
            }
        }
    }

    public void SetCurrentGameState(GameState newState)
    {
        currentGameState = newState;
        UpdateGameState();

        // has anybody subscribed to this event? if so broadcast event
        OnGameStateChanged?.Invoke(newState);
    }

    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }
    
    public void SetCurrentPlayer(CurrentPlayer newPlayer)
    {
        currentPlayer = newPlayer;
    }

    public CurrentPlayer GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public WinCondition GetWinCondition()
    {
        return winCondition;
    }

    public void SetWinCondition(WinCondition newWinCondition)
    {
        this.winCondition = newWinCondition; 
    }

    public void StartGame()
    {
        SetCurrentGameState(GameState.PLAYING);
    }

    // maybe use SetCurrentPlayer instead?
    public void ChangePlayer()
    {
        currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1) ? CurrentPlayer.PLAYER_2 : CurrentPlayer.PLAYER_1;
    }
}

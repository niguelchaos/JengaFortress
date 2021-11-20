using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MAIN_MENU,
    PLAYER_1,
    PLAYER_2,
    GAME_OVER
}

public enum WinCondition {HitFloor, LeaveBoundary, Both}


public class GameManager : MonoBehaviour
{
    // basically a singleton - only 1, accessible anywhere, instance can be retrieved anywhere
    public static GameManager Instance;

    [SerializeField] private GameState currentGameState;
    [SerializeField] private WinCondition winCondition;

    public static event Action<GameState> OnGameStateChanged;

    private float timer;
    [SerializeField] private float changeStateTime = 1;
    


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // set to main menu
        SetCurrentGameState(GameState.MAIN_MENU);
        timer = changeStateTime;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        } 
        else if (timer <= 0)
        {
            switch(currentGameState)
            {
                case GameState.MAIN_MENU:
                    SetCurrentGameState(GameState.PLAYER_1);
                    break;
                case GameState.PLAYER_1:
                    SetCurrentGameState(GameState.PLAYER_2);
                    break;
                case GameState.PLAYER_2:
                    SetCurrentGameState(GameState.PLAYER_1);
                    break;
            }
            timer = changeStateTime;
        }
    }
    private void UpdateGameState()
    {
        switch(currentGameState)
        {
            case GameState.MAIN_MENU:
                break;
            case GameState.PLAYER_1:
                break;
            case GameState.PLAYER_2:
                break;
            case GameState.GAME_OVER:
                break;
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
    { return currentGameState; }
    
    public WinCondition GetWinCondition() { return winCondition; }
    public void SetWinCondition(WinCondition newWinCondition) 
    { this.winCondition = newWinCondition;  }
}

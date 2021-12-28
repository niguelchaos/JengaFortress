using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    MAIN_MENU,
    SETUP,
    PLACE_FORTRESS,
    BATTLE,
    PLAYING,
    PAUSED,
    GAME_OVER
}

// TODO: dont care about this for now
public enum PlayingState
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

public enum WinCondition { HitFloor, LeaveBoundary, Both }


public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager Instance;

    // States
    [SerializeField] private GameState gameState;
    [SerializeField] private PlayingState playingState;
    [SerializeField] private CurrentPlayer _currentPlayer;
    [SerializeField] private WinCondition winCondition;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<PlayingState> OnPlayingStateChanged;

    // 
    [SerializeField] private GameObject gameStateCube;
    private Renderer cubeRenderer;
    

    private void Awake()
    {
        Instance = this;
        SetGameState(GameState.SETUP);
        SetPlayingState(PlayingState.START_TURN);
        //SetCurrentPlayer(CurrentPlayer.PLAYER_1);
        currentPlayer = CurrentPlayer.PLAYER_1;
    }

    private void Start()
    {
        // set to main menu
        gameStateCube = GameObject.Find("GameStateCube");
        if (gameStateCube != null)
        {
            cubeRenderer = gameStateCube.GetComponent<Renderer>();
        }
    }

    //private void Update() {}

    private void UpdateGameState()
    {
        if (cubeRenderer != null)
        {
            switch(gameState)
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

    private void UpdatePlayingState()
    {
        switch(playingState)
        {
            case PlayingState.START_TURN:
                currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1)
                                    ? CurrentPlayer.PLAYER_2
                                    : CurrentPlayer.PLAYER_1;
                break;
            // case PlayingState.THROWING | PlayingState.AIMING:
            //     break;
            // case PlayingState.END_TURN:
            //     break;
        }
    }


    public void SetGameState(GameState newState)
    {
        gameState = newState;
        UpdateGameState();

        // has anybody subscribed to this event? if so broadcast event
        OnGameStateChanged?.Invoke(newState);
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetPlayingState(PlayingState newState)
    {
        playingState = newState;
        UpdatePlayingState();

        // has anybody subscribed to this event? if so broadcast event
        OnPlayingStateChanged?.Invoke(newState);
    }

    public PlayingState GetPlayingState()
    {
        return playingState;
    }
    
    // public void SetCurrentPlayer(CurrentPlayer newPlayer)
    // {
    //     _currentPlayer = newPlayer;
    // }

    // public CurrentPlayer GetCurrentPlayer()
    // {
    //     return _currentPlayer;
    // }

    public CurrentPlayer currentPlayer
    {
        get { return _currentPlayer; }
        set { _currentPlayer = value; }
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
        SetGameState(GameState.PLAYING);
    }
    
    public void ChangePlayer()
    {
        SetPlayingState(PlayingState.START_TURN);
        //SetCurrentPlayer((CurrentPlayer.PLAYER_1 & CurrentPlayer.PLAYER_2) ^ currentPlayer);

        // TODO: maybe use SetCurrentPlayer instead?
        //currentPlayer = (currentPlayer == CurrentPlayer.PLAYER_1) ? CurrentPlayer.PLAYER_2 : CurrentPlayer.PLAYER_1;
    }

}

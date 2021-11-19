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

public class GameManager : MonoBehaviour
{
    // basically a singleton - only 1, accessible anywhere, instance can be retrieved anywhere
    public static GameManager Instance;

    [SerializeField] private GameState currentGameState;

    private void Awake()
    {
        Instance = this;
    }
    public void SetCurrentGameState(GameState newState)
    {
        currentGameState = newState;
    }
}

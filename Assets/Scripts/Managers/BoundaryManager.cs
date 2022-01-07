using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    [SerializeField] private GameObject playerBoundaryPrefab;
    private PlayerBoundary playerBoundary_P1, playerBoundary_P2;

    private GameObject sessionOrigin;
    [SerializeField] private GameObject groundPlaneGO;

    void Start()
    {
        sessionOrigin = GameObject.Find("AR Session Origin");

        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
        //GameManager.OnCurrentPlayerChanged += UpdateOnCurrentPlayerChanged;
    }

    private void UpdateOnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.SET_BOUNDARIES)
        {
            groundPlaneGO = sessionOrigin.GetComponent<Setup>().groundPlane;

            playerBoundary_P1 = Instantiate(playerBoundaryPrefab).GetComponent<PlayerBoundary>();
            playerBoundary_P2 = Instantiate(playerBoundaryPrefab).GetComponent<PlayerBoundary>();

            playerBoundary_P1.player = CurrentPlayer.PLAYER_1;
            playerBoundary_P2.player = CurrentPlayer.PLAYER_2;

            playerBoundary_P1.SetBoundaryTransform(groundPlaneGO);
            playerBoundary_P2.SetBoundaryTransform(groundPlaneGO);
            
            GameManager.Instance.SetGameState(GameState.PLACE_FORTRESS);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    [SerializeField] private Player player;
    private BoxCollider boundaryCollider;

    // The objects that are in the player's boundary
    // todo: not sure what object to use as reference point
    private string p1GameObjName = "Fortress_P1";
    private string p2GameObjName = "Fortress_P2";

    [SerializeField] private GameObject playerBoundaryCenter;
    private MeshRenderer meshRenderer;
    private bool isAttached = false;

    void Start()
    {
        boundaryCollider = GetComponent<BoxCollider>();

        meshRenderer = GetComponent<MeshRenderer>();
        player = gameObject.transform.parent.gameObject.GetComponent<Player>(); // todo: vettefan..
        
        GetPlayerBoundaryCenter();
        StartCoroutine(WaitToAttach(2));

        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
        GameManager.OnCurrentPlayerChanged += UpdateOnCurrentPlayerChanged;
    }

    private void OnEnable()
    {
        isAttached = false;
        StartCoroutine(WaitToAttach(2));
    }
    private void OnDisable()
    {
        isAttached = false;
    }

    private void UpdateOnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.PLAYING)
        {
            isAttached = false;
            StartCoroutine(WaitToAttach(2));
        }
        CheckMeshRenderer();
    }
    private void UpdateOnCurrentPlayerChanged(CurrentPlayer currentPlayer)
    {
        transform.position = playerBoundaryCenter.transform.position;
        if (GameManager.Instance.GetGameState() == GameState.PLAYING)
        {
            isAttached = true;
        }
    }

    // todo: could this instead be in player.cs?
    private void GetPlayerBoundaryCenter()
    {
        switch(player.GetPlayerNum())
        {
            case PlayerNum.P1:
                playerBoundaryCenter = GameObject.Find(p1GameObjName);
                break;
            case PlayerNum.P2:
                playerBoundaryCenter = GameObject.Find(p2GameObjName);
                break;
        }
    }
    
    private void AttachToStartingPoint()
    {
        transform.position = playerBoundaryCenter.transform.position;
        isAttached = true;
    }

    IEnumerator WaitToAttach(int time)
    {
        yield return new WaitForSeconds(time);
        AttachToStartingPoint();
    }

    public void CheckMeshRenderer()
    {
        DisableRenderer();
        switch (GameManager.Instance.currentPlayer, player.GetPlayerNum())
        {
            case (CurrentPlayer.PLAYER_1, PlayerNum.P1):
            case (CurrentPlayer.PLAYER_2, PlayerNum.P2):
                EnableRenderer();
                break;
        }
    }

    public void EnableRenderer()
    {
        meshRenderer.enabled = true;
    }
    public void DisableRenderer()
    {
        meshRenderer.enabled = false;
    }

}

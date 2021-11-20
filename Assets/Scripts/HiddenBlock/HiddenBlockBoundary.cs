using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenBlockBoundary : MonoBehaviour
{
    [SerializeField] private Player player;
    private SphereCollider boundaryCollider;

    private string p1GameObjName = "HiddenBlock_P1";
    private string p2GameObjName = "HiddenBlock_P2";

   [SerializeField]  private GameObject playerHbGO;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        boundaryCollider = GetComponent<SphereCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        player = gameObject.transform.parent.gameObject.GetComponent<Player>();

        GetPlayerHbGO();
        StartCoroutine(WaitToAttach(2));
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
    }

    private void UpdateOnGameStateChanged(GameState currentGameState)
    {
        CheckMeshRenderer();
    }

    public void CheckMeshRenderer()
    {
        DisableRenderer();
        switch (GameManager.Instance.GetCurrentGameState(), player.GetPlayerNum())
        {
            case (GameState.PLAYER_1, PlayerNum.P1):
            case (GameState.PLAYER_2, PlayerNum.P2):
                EnableRenderer();
                break;
        }
    }


    private void GetPlayerHbGO()
    {
        switch(player.GetPlayerNum())
        {
            case PlayerNum.P1:
                playerHbGO = GameObject.Find(p1GameObjName);
                break;
            case PlayerNum.P2:
                playerHbGO = GameObject.Find(p2GameObjName);
                break;
        }
    }
    private void AttachToStartingPoint()
    {
        transform.position = playerHbGO.transform.position;
    }
    
    IEnumerator WaitToAttach(int time)
    {
        // print("Waiting");
        yield return new WaitForSeconds(time);
        AttachToStartingPoint();
    }

    private void OnTriggerExit(Collider col)
    {
        HiddenCoreBlock hiddenCoreBlock = playerHbGO.GetComponent<HiddenCoreBlock>();
        if (col.gameObject.tag == "CoreBlock")
        {
            Debug.Log("Left Boundary");
            if (GameManager.Instance.GetWinCondition() != WinCondition.HitFloor)
            {
                GameManager.Instance.SetCurrentGameState(GameState.GAME_OVER);
            }
        }
    }

    public void EnableRenderer(){   meshRenderer.enabled = true;    }
    public void DisableRenderer(){   meshRenderer.enabled = false;    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    [SerializeField] private Player player;
    private BoxCollider boundaryCollider;
    [SerializeField] private float boundaryOffset = 5.0f;
    //private Vector3 boundaryPos_P1, boundaryPos_P2;
    public bool isWithinBoundary { get; set; } // todo: would it be logical to have canInteract() eller något i player.cs?
    
    [SerializeField] private GameObject groundPlaneGO;
    private string groundPlaneGameObjName = "GroundPlane";

    private MeshRenderer meshRenderer;
    private bool isAttached = false;

    void Start()
    {
        boundaryCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        //player = gameObject.transform.parent.gameObject.GetComponent<Player>(); // todo: TODO

        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
        GameManager.OnCurrentPlayerChanged += UpdateOnCurrentPlayerChanged;
    }

    private void OnEnable()
    {
        isAttached = false;

    }
    private void OnDisable()
    {
        isAttached = false;
    }

    private void UpdateOnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.SET_BOUNDARIES)
        {
            //groundPlaneGO = Transform.Find("GroundPlane");
            SetBoundaryTransform();
        }
        CheckMeshRenderer();
    }
    private void UpdateOnCurrentPlayerChanged(CurrentPlayer currentPlayer)
    {
        //transform.position = playerBoundaryCenter.transform.position; // todo: TODO
        if (GameManager.Instance.GetGameState() == GameState.PLAYING)
        {
            isAttached = true;
        }
    }

    private void SetBoundaryTransform()
    {
        GameObject groundPlaneGO = GameObject.Find(groundPlaneGameObjName);

        // todo: Ska det va ett skript för varje spelare eller inte?
        // todo: whatt to do?
        boundaryCollider.size = new Vector3(
            groundPlaneGO.transform.localScale.x / 2 - boundaryOffset * 2,
            25f,
            groundPlaneGO.transform.localScale.z - boundaryOffset * 2
        );

        transform.position = new Vector3(
            boundaryOffset + (player.GetPlayerNum() is PlayerNum.P1 ? 0 :
                groundPlaneGO.transform.position.x),
            0f,
            boundaryOffset
        );

        // switch (player.GetPlayerNum()) {
        // case PlayerNum.P1:
        //     boundaryPos_P1 = new Vector3 {
        //         boundaryOffset, 0, boundaryOffset
        //     };
        //     break;
        // case PlayerNum.P2:
        //     boundaryPos_P2 = new Vector3 {
        //         groundPlaneGO.transform.position.x + boundaryOffset, 0, boundaryOffset
        //     };
        //     break;
        // }
    }
    
    // private void AttachToStartingPoint()
    // {
    //     transform.position = playerBoundaryCenter.transform.position;
    //     isAttached = true;
    // }

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

    private void OnTriggerEnter(Collider other) {
        isWithinBoundary = true;
    }
    private void OnTriggerExit(Collider other) {
        isWithinBoundary = false;
    }

}

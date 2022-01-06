using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    [SerializeField] private Player player;
    
    private BoxCollider boundaryCollider;
    [SerializeField] private float boundaryOffset = 5.f;
    //private Vector3 boundaryPos_P1, boundaryPos_P2;
    private bool isWithinBoundary { get; } // todo: would it be logical to have canInteract() eller något i player.cs?
    private string groundPlaneGameObjName = "GroundPlane";


    //[SerializeField] private GameObject groundPlaneGO;
    private MeshRenderer meshRenderer;

    void Start()
    {
        boundaryCollider = GetComponent<BoxCollider>();

        meshRenderer = GetComponent<MeshRenderer>();
        //player = gameObject.transform.parent.gameObject.GetComponent<Player>(); // todo: vettefan..
        
        //GetPlayerBoundaryCenter();
        // StartCoroutine(WaitToAttach(2));

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
            SetBoundaryTransforms();
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

    private void SetBoundaryTransforms()
    {
        GameObject groundPlaneGO = GameObject.Find(groundPlaneGameObjName);
        
        boundaryCollider.transform.scale = new Vector3 {
            groundPlaneGO.transform.scale.x / 2 - boundaryOffset * 2,
            0,
            groundPlaneGO.transform.scale.z - boundaryOffset * 2
        };

        boundaryCollider.transform.position = new Vector3 {
            boundaryOffset + (player.GetPlayerNum() is PlayerNum.P1 ? 0 :
                groundPlaneGO.transform.position.x),
            0,
            boundaryOffset
        };

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

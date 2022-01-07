using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    //[SerializeField] private Player player;
    [SerializeField] public CurrentPlayer player { get; set; }
    private BoxCollider boundaryCollider;
    [SerializeField] private float boundaryOffset = 5.0f;
    //private Vector3 boundaryPos_P1, boundaryPos_P2;
    public bool isWithinBoundary { get; private set; }
    // [SerializeField] private GameObject groundPlaneGO;
    // private GameObject sessionOrigin;
    // private Setup setup;

    private MeshRenderer meshRenderer;

    void Start()
    {
        boundaryCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        // sessionOrigin = GameObject.Find("AR Session Origin");
        // setup = sessionOrigin.GetComponent<Setup>();
        
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
    }

    private void UpdateOnGameStateChanged(GameState gameState)
    {
        // if (gameState == GameState.SET_BOUNDARIES)
        //     SetBoundaryTransform();
        CheckMeshRenderer();
    }

    public void SetBoundaryTransform(GameObject groundPlaneGO)
    {
        //GameObject groundPlaneGO = setup.groundPlane;

        Debug.Log("groundPlaneGO.transform.localScale: " + groundPlaneGO.transform.localScale);
        Debug.Log("groundPlaneGO.transform.localPosition: " + groundPlaneGO.transform.localPosition);
        Debug.Log("groundPlaneGO.transform.position: " + groundPlaneGO.transform.localPosition);

        // boundaryCollider.size = new Vector3 (
        transform.localScale = new Vector3 (
            groundPlaneGO.transform.localScale.x / 2 - boundaryOffset * 2,
            25f,
            groundPlaneGO.transform.localScale.z - boundaryOffset * 2
        );

        if (player == CurrentPlayer.PLAYER_1) {
            transform.position = groundPlaneGO.transform.position + new Vector3 (
                - groundPlaneGO.transform.localScale.x / 2 + boundaryOffset,
                0f,
                - groundPlaneGO.transform.localScale.z / 2 + boundaryOffset
            );
        } else if (player == CurrentPlayer.PLAYER_2) {
            transform.position = groundPlaneGO.transform.position + new Vector3 (
                boundaryOffset,
                0f,
                - groundPlaneGO.transform.localScale.z / 2 + boundaryOffset
            );
        }

        // transform.position = new Vector3(
        //     boundaryOffset + (player == CurrentPlayer.PLAYER_1 ? 0 : groundPlaneGO.transform.position.x),
        //     0f,
        //     boundaryOffset
        // );
    }

    public void CheckMeshRenderer()
    {
        DisableRenderer();
        if (GameManager.Instance.currentPlayer == player)
        {
            EnableRenderer();
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
        if (other.gameObject.tag == "Player") {
            isWithinBoundary = true;
            Debug.Log("OnTriggerExit: " + other.gameObject.name);
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            isWithinBoundary = false;
            Debug.Log("OnTriggerExit: " + other.gameObject.name);
        }
    }

}

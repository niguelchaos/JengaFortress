using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;


public enum PlaceCoreBlockMode
{
    PLACE, 
    ADJUST
}

public class PlaceCoreBlock: MonoBehaviour 
{
    private Camera myCamera;
    private SessionOriginController sessionController;
    private ARRaycastManager raycastManager;
    public GameObject content;

    public GameObject coreBlockCanvas;
    public GameObject mainCanvas;
    public GameObject playCanvas;

    public TMP_Text currentModeText;
    public TMP_Text currentPlayerText;
    
    private PlaceCoreBlockMode coreBlockMode;
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    private Vector2 touchPosition;

    private GameObject currentPlayer; 
    private GameObject player1; 
    private GameObject player2;

    private GameObject coreBlockP1;
    private GameObject coreBlockP2;

    private GameObject HBBoundaryP1;
    private GameObject HBBoundaryP2;


    // private void Awake()
    // {
    // }

    private void Start() {
        myCamera = 
        	this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();

        raycastManager = this.gameObject.GetComponent<ARRaycastManager>();
        sessionController = this.gameObject.GetComponent<SessionOriginController>();
          
        InputManager.Instance.OnFirstTouch += CheckTouchAction;
        // subscribe to gamestate changes
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;

    }

    private void Update()
    {
        // CheckUI();
    }

    private void UpdateOnGameStateChanged(GameState currentGameState)
    {
        CheckUI();
    }

    private void CheckUI()
    {
        if (GameManager.Instance.GetGameState() == GameState.PLACE_CORE_BLOCK)
        {
            mainCanvas.SetActive(false);
            coreBlockCanvas.SetActive(true);
            UpdateUI();
        }
    }
    private void BackToMainCanvas()
    {
        mainCanvas.SetActive(true);
        coreBlockCanvas.SetActive(false);
        playCanvas.SetActive(true);
    }

    private void CheckTouchAction(Touch touch)
    {
        if (GameManager.Instance.GetGameState() == GameState.PLACE_CORE_BLOCK)
        {
            // bool ARhit;
            // ARRaycastHit nearestHitPose = new ARRaycastHit();

            // ARhit = raycastManager.Raycast(screenPosition, myARHits,
            //     TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon);
            /////////////////////////////////////////////////////////////////////

            RaycastHit[] hits;
            Ray ray;
            RaycastHit nearestHit;

            Vector2 screenPosition = touch.position;

            if (sessionController.IsPointOverUIObject(screenPosition))
            {
                // logger.Log ("clicked on button");
                return;
            }
            /////////////////////////////////////////////////////////////////////
            // if (ARhit == true) {
            //     logger.Log("Hit: " + ARhit);
            //     nearestHitPose = myARHits[0];
            // }


            ray = myCamera.ScreenPointToRay(screenPosition);
            bool rayHit = Physics.Raycast(ray, out nearestHit);
            if (rayHit)
            {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            }  

            hits = Physics.RaycastAll(ray);


            switch (coreBlockMode)
            {
                case PlaceCoreBlockMode.PLACE:
                    CheckPlace(screenPosition, rayHit, nearestHit);
                    break;

                case PlaceCoreBlockMode.ADJUST:
                    break;
            }
        }  
    }

    private void CheckPlace(Vector2 screenPosition, bool rayHit, RaycastHit nearestHit)
    {
        GameObject currentPlayerPrefab;

        if (GameManager.Instance.currentPlayer == CurrentPlayer.PLAYER_1)
        {   currentPlayerPrefab = player1Prefab;    }
        else 
        {   currentPlayerPrefab = player2Prefab;    }

        if (rayHit)
        {
            if (nearestHit.transform.gameObject.layer == LayerManager.GroundLayer ||
                nearestHit.transform.gameObject.layer == LayerManager.BlockLayer) {
                if (currentPlayer != null)
                {
                    AdjustPlayerBlock(nearestHit);
                } else {
                    SpawnPlayerBlock(currentPlayerPrefab, nearestHit);
                }
                return;
            }
        }
    }
    private void CheckAdjust()
    {

    }

    private void SpawnPlayerBlock(GameObject currentPlayerPrefab, RaycastHit nearestHit)
    {
        if (currentPlayer == null)
        {
            Vector3 spawnPoint = new Vector3(nearestHit.point.x, nearestHit.point.y + 1, nearestHit.point.z);
            currentPlayer = Instantiate(currentPlayerPrefab, spawnPoint, Quaternion.identity);

            if (GameManager.Instance.currentPlayer == CurrentPlayer.PLAYER_1)
            {
                coreBlockP1 = currentPlayer.transform.Find("HiddenBlock_P1").gameObject;
                HBBoundaryP1 = currentPlayer.transform.Find("HBBoundary_P1").gameObject;
                // HBBoundaryP1.SetActive(false);
            }
            else 
            {
                coreBlockP2 = currentPlayer.transform.Find("HiddenBlock_P2").gameObject;
                HBBoundaryP2 = currentPlayer.transform.Find("HBBoundary_P2").gameObject;
                // HBBoundaryP2.SetActive(false);
            }
            // SetObjectIsKinematic(spawnedCoreBlock, true);

            currentPlayer.transform.parent = content.transform;
        }
        else {
            // Debug.Log("Only 1 Core Block per player");
        }
    }

    private void AdjustPlayerBlock(RaycastHit nearestHit)
    {
        Vector3 adjustPos = new Vector3(nearestHit.point.x, nearestHit.point.y + 1, nearestHit.point.z);
        if (GameManager.Instance.currentPlayer == CurrentPlayer.PLAYER_1)
        {
            coreBlockP1.transform.position = adjustPos;
        }
        else {
            coreBlockP2.transform.position = adjustPos;
        }
    }

    private void SetObjectIsKinematic(GameObject spawnedObject, bool isKOrNotIdkMan)
    {
        Rigidbody[] rbs = spawnedObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = isKOrNotIdkMan;
        }
    }

    public void ConfirmPlacement()
    {
        if (currentPlayer == null)
        {
            Debug.Log("Must place core block before moving on");
            return;
        }

        if (GameManager.Instance.currentPlayer == CurrentPlayer.PLAYER_1)
        {
            GameManager.Instance.currentPlayer = CurrentPlayer.PLAYER_2;
            player1 = currentPlayer; // remember p1 core block prefab
            currentPlayer = null;
            UpdateUI();
        }
        // must be player 2
        // set both to non-kinematic, return to p1
        else {
            player2 = currentPlayer;
            // HBBoundaryP1.SetActive(true);
            // HBBoundaryP2.SetActive(true);
            GameManager.Instance.SetGameState(GameState.PLAYING);
            GameManager.Instance.currentPlayer = CurrentPlayer.PLAYER_1;
            Debug.Log("back to playing state");
            BackToMainCanvas();
        }

    }

    private void UpdateUI()
    {
        currentModeText.text = coreBlockMode.ToString();
        currentPlayerText.text = GameManager.Instance.currentPlayer.ToString();
    }

}
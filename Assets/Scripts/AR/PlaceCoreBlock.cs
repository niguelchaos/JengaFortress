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

    public TMP_Text currentModeText;
    public TMP_Text currentPlayerText;
    
    private PlaceCoreBlockMode coreBlockMode;
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    private Vector2 touchPosition;
    private GameObject p1SpawnedCoreBlock;
    private GameObject spawnedCoreBlock;

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
        if (GameManager.Instance.GetCurrentGameState() == GameState.PLACE_CORE_BLOCK)
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
    }

    private void CheckTouchAction(Touch touch)
    {
        if (GameManager.Instance.GetCurrentGameState() == GameState.PLACE_CORE_BLOCK)
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

        if (GameManager.Instance.GetCurrentPlayer() == CurrentPlayer.PLAYER_1)
        {   currentPlayerPrefab = player1Prefab;    }
        else 
        {   currentPlayerPrefab = player2Prefab;    }

        if (rayHit)
        {
            if (nearestHit.transform.gameObject.layer == LayerManager.GroundLayer ||
                nearestHit.transform.gameObject.layer == LayerManager.BlockLayer) {
                SpawnPlayerBlock(currentPlayerPrefab, nearestHit);
                return;
            }
        }
    }
    private void CheckAdjust()
    {

    }

    private void SpawnPlayerBlock(GameObject currentPlayerPrefab, RaycastHit nearestHit)
    {
        if (spawnedCoreBlock == null)
        {
            Vector3 spawnPoint = new Vector3(nearestHit.point.x, nearestHit.point.y + 1, nearestHit.point.z);
            spawnedCoreBlock = Instantiate(currentPlayerPrefab, spawnPoint, Quaternion.identity);
            SetObjectIsKinematic(spawnedCoreBlock, true);

            spawnedCoreBlock.transform.parent = content.transform;
        }
        else {
            Debug.Log("Only 1 Core Block per player");
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
        if (spawnedCoreBlock == null)
        {
            Debug.Log("Must place core block before moving on");
            return;
        }

        if (GameManager.Instance.GetCurrentPlayer() == CurrentPlayer.PLAYER_1)
        {
            GameManager.Instance.SetCurrentPlayer(CurrentPlayer.PLAYER_2);
            p1SpawnedCoreBlock = spawnedCoreBlock; // remember p1 core block prefab
            spawnedCoreBlock = null;
            UpdateUI();
        }
        // must be player 2
        // set both to non-kinematic, return to p1
        else {
            GameManager.Instance.SetCurrentGameState(GameState.PLAYING);
            SetObjectIsKinematic(p1SpawnedCoreBlock, false);
            SetObjectIsKinematic(spawnedCoreBlock, false);
            GameManager.Instance.SetCurrentPlayer(CurrentPlayer.PLAYER_1);
            BackToMainCanvas();
        }

    }

    private void UpdateUI()
    {
        currentModeText.text = coreBlockMode.ToString();
        currentPlayerText.text = GameManager.Instance.GetCurrentPlayer().ToString();
    }

}
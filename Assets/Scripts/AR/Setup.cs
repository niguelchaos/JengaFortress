using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;


public enum SetupPhase
{
    GROUNDPLANE,
    FIRING_POSITION
}

public class Setup: MonoBehaviour 
{
    private SetupPhase setupPhase;
    private ARSessionOrigin arSessionOrigin;
    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    private ARFireProjectile fireProjectile;
    private SessionOriginController sessionController;
    private Camera myCamera;

    public TMP_Text currentModeText;

    public GameObject groundPlane { get; set; }

    public GameObject spawnGroundReticle;
    public GameObject groundPlanePrefab;

    public GameObject content;

    public GameObject mainCanvas;
    public GameObject setupCanvas;
    public GameObject groundplaneCanvas;
    public GameObject firingposCanvas;
    public Slider firingPosSlider;

    public GameObject firingPos;
    public Vector3 selectedFirePosition;



    private void Awake()
    {
        arSessionOrigin = GetComponent<ARSessionOrigin>();
        myCamera = this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();
        raycastManager = this.gameObject.GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        sessionController = this.gameObject.GetComponent<SessionOriginController>();
        fireProjectile = GetComponent<ARFireProjectile>();
    }

    void Start() 
    {
        InputManager.Instance.OnFirstTouch += CheckTouchAction;
        // subscribe to gamestate changes
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
        CheckUI();
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
        // print("checking ui");
        if (GameManager.Instance.GetGameState() == GameState.SETUP)
        {
            mainCanvas.SetActive(false);
            setupCanvas.SetActive(true);
            switch (setupPhase)
            {
                case SetupPhase.GROUNDPLANE:
                    groundplaneCanvas.SetActive(true);
                    firingposCanvas.SetActive(false);
                    break;
                case SetupPhase.FIRING_POSITION:
                    groundplaneCanvas.SetActive(false);
                    firingposCanvas.SetActive(true);
                    break;
            }
            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        currentModeText.text = setupPhase.ToString();
    }

    private void CheckTouchAction(Touch touch)
    {
        if (GameManager.Instance.GetGameState() == GameState.SETUP)
        {
            bool ARhit;
            ARRaycastHit nearestHitPose = new ARRaycastHit();
            List<ARRaycastHit> myARHits = new List <ARRaycastHit>();
            /////////////////////////////////////////////////////////////////////
            RaycastHit[] hits;
            Ray ray;
            RaycastHit nearestHit;
            Vector2 screenPosition = touch.position;

            if (sessionController.IsPointOverUIObject(screenPosition))
            {
                // Debug.Log ("clicked on button");
                return;
            }

            ARhit = raycastManager.Raycast(screenPosition, myARHits,
                TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon);
            
            ray = myCamera.ScreenPointToRay(screenPosition);
            bool rayHit = Physics.Raycast(ray, out nearestHit);

            if (ARhit == true) {
                nearestHitPose = myARHits[0];
            }

            if (rayHit)
            {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            }  

            hits = Physics.RaycastAll(ray);
            /////////////////////////////////////////////////////////////////////
            
            switch (setupPhase)
            {
                case SetupPhase.GROUNDPLANE:
                    CheckGroundPlane(screenPosition, ARhit, nearestHitPose, rayHit, nearestHit);
                    break;
                case SetupPhase.FIRING_POSITION:
                    break;
            }

        }
    }

    private void CheckGroundPlane(Vector2 screenPosition, bool ARhit, ARRaycastHit nearestHitPose, bool rayHit, RaycastHit nearestHit)
    {
        if (ARhit)
        {
            spawnGroundReticle.transform.position = nearestHitPose.pose.position;
        }
        else if (rayHit)
        {
            spawnGroundReticle.transform.position = nearestHit.point;
        }
    }


    public void ConfirmFinishSetup()
    {
        if (setupPhase == SetupPhase.GROUNDPLANE)
        {
            if (groundPlane == null)
            {
                Debug.Log("Must place ground plane before moving on");
                return;
            }
            else {
                setupPhase = SetupPhase.FIRING_POSITION;
            }
            CheckUI();
        }
        else if (setupPhase == SetupPhase.FIRING_POSITION)
        {
            if (fireProjectile.fireSpawnDist == 0)
            {
                Debug.Log("Must set firing pos");
                return;
            }
            else {   
                GameManager.Instance.SetGameState(GameState.SET_BOUNDARIES);
                // GameManager.Instance.SetGameState(GameState.PLACE_FORTRESS);
                BackToMainCanvas();
            }
        }
    }

    ///////////////////////////////// Firing pos ///////////////////////////////
    // used by slider
    public void SetFiringPos()
    {
        firingPos.transform.position = myCamera.gameObject.transform.position + (myCamera.transform.forward * firingPosSlider.value);
    }

    // used by setfirepos button
    public void SetInitFirePosition()
    {
        selectedFirePosition = firingPos.transform.position;
        Debug.Log("Remembering inital position:  " + selectedFirePosition);

        // calculate distance between camera and firingpos, set it
        fireProjectile.fireSpawnDist = GetInitFireSpawnDist();
        
        ConfirmFinishSetup();
    }

    public float GetInitFireSpawnDist()
    {
        float distance = Vector3.Distance(selectedFirePosition, myCamera.gameObject.transform.position);
        Debug.Log("init spawn dist: " + distance);
        return distance;
    }
    ////////////////////////////// Ground Plane ///////////////////////////////////////
    public void PlaceGroundPlaneOnReticle()
    {
        PlaceGroundPlane(spawnGroundReticle.transform.position);
    }

    public void PlaceGroundPlane(Vector3 selectedPos)
    {
        Debug.Log ("ground pos:  " + selectedPos);
        groundPlane = Instantiate(groundPlanePrefab, selectedPos, Quaternion.identity);

        groundPlane.AddComponent<ARAnchor>();
        // placeFortress.groundPlane.transform.parent = arOrigin.transform;

        content.transform.position = groundPlane.transform.position;
        Debug.Log ("groundplane pos:  " + groundPlane.transform.position);
        
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;
        planeManager.enabled = false;
        Debug.Log ("planes disabled");
        
        ConfirmFinishSetup();
    }
    /////////////////////////////////////////////////////////////////////

    public void BackToMainCanvas()
    {
        mainCanvas.SetActive(true);
        setupCanvas.SetActive(false);
        groundplaneCanvas.SetActive(false);
        firingposCanvas.SetActive(false);
    }

}
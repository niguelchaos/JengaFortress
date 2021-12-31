using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public enum PlaceMode
{
    PLACE, 
    SELECT,
    MOVE,
    FIRE
}

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceFortress: MonoBehaviour {

    [SerializeField] private PlaceMode placeMode;
    public ARPlaneManager planeManager;
    private ARAnchorManager anc;
    private ARSessionOrigin arSessionOrigin;
    private ARRaycastManager raycastManager;
    public Camera myCamera;

    private SessionOriginController sessionController;
    private Setup setup;

    public GameObject mainCanvas;
    public GameObject placefortressCanvas;
    public TMP_Text currentPlayerText;
    public TMP_Text currentModeText;

    public GameObject fortressPrefab;
    private Vector2 touchPosition;

    static List<ARRaycastHit> myARHits = new List <ARRaycastHit>();
    private GameObject foundObject = null;
    public float cooldown, cooldownCount;
    private GameObject p1SpawnedFortress;
    private GameObject spawnedFortress;
    

    private static ILogger logger = Debug.unityLogger;

    public PlaceMode GetPlaceMode() {   return placeMode;   }

    private void Awake()
    {
        arSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    private void Start() {
        cooldown = 2;
        myCamera = 
        	this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();

        raycastManager = this.gameObject.GetComponent<ARRaycastManager>();
        anc = this.gameObject.GetComponent<ARAnchorManager>();
        planeManager = this.gameObject.GetComponent<ARPlaneManager>();
        sessionController = this.gameObject.GetComponent<SessionOriginController>();
        setup = GetComponent<Setup>();
           
        InputManager.Instance.OnFirstTouch += CheckTouchAction;
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
    }

    void Update() {
        if (cooldownCount <= 2)
        {
            cooldownCount += Time.deltaTime;
        }
    }

    private void UpdateOnGameStateChanged(GameState currentGameState)
    {
        CheckUI();
    }

    private void CheckUI()
    {
        if (GameManager.Instance.GetGameState() == GameState.PLACE_FORTRESS)
        {
            mainCanvas.SetActive(false);
            placefortressCanvas.SetActive(true);
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        currentModeText.text = placeMode.ToString();
        currentPlayerText.text = GameManager.Instance.currentPlayer.ToString();
    }
    
    private void BackToMainCanvas()
    {
        mainCanvas.SetActive(true);
        placefortressCanvas.SetActive(false);
    }

    private void CheckTouchAction(Touch touch)
    {
        if (GameManager.Instance.GetGameState() == GameState.PLACE_FORTRESS)
        {
            //////////////////////////// Basic Raycast ///////////////////////////////
            bool ARhit;
            ARRaycastHit nearestHitPose = new ARRaycastHit();

            RaycastHit[] hits;
            Ray ray;

            Vector2 screenPosition = touch.position;
            // Debug.Log ("position:  " + screenPosition);

            if (sessionController.IsPointOverUIObject(screenPosition))
            {
                // logger.Log ("clicked on button");
                return;
            }
            /////////////////////////////////////////////////////////////////////

            ARhit = raycastManager.Raycast(screenPosition, myARHits,
                TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon);
        

            if (ARhit == true) {
                // logger.Log("Hit: " + ARhit);
                nearestHitPose = myARHits[0];
            }

            RaycastHit nearestHit;
            ray = myCamera.ScreenPointToRay(screenPosition);
            bool rayHit = Physics.Raycast(ray, out nearestHit);
            if (rayHit)
            {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            }  

            hits = Physics.RaycastAll(ray);
            /////////////////////////////////////////////////////////////////////
            
            switch (placeMode)
            {
                case PlaceMode.PLACE:
                    CheckPlace(rayHit, ARhit, nearestHit, nearestHitPose, screenPosition);
                    foundObject = null;
                    break;
                
                case PlaceMode.SELECT:
                    CheckSelect(rayHit, ARhit, nearestHit, nearestHitPose, screenPosition);
                    break;
                
                case PlaceMode.MOVE:
                    if (foundObject == null)
                    {
                        Debug.Log("Nothing Selected");
                        break;
                    }
                    else {
                        CheckMove(rayHit, ARhit, nearestHit, nearestHitPose, screenPosition);   
                    }
                    break;
                
                case PlaceMode.FIRE:
                    break;
            }
        }
    }

    private void CheckPlace(bool rayHit, bool ARhit, RaycastHit nearestHit, ARRaycastHit nearestHitPose, Vector2 screenPosition)
    {
        if (spawnedFortress != null)
        {
            Debug.Log("Can only spawn 1 fortress per player");
            return;
        }

        if (planeManager.enabled == false || planeManager.requestedDetectionMode == PlaneDetectionMode.None)
        {
            // logger.Log ("plane manager disabled");
            if (rayHit)
            {
                if (nearestHit.transform.gameObject.layer == LayerManager.GroundLayer) {
                    CheckSpawnFortress(screenPosition, nearestHitPose, nearestHit, false);
                    return;
                }
            }
        }
        // old code - basically accounts for placing fortresses on AR planes too
        else if (ARhit == true)
        {
            CheckSpawnFortress(screenPosition, nearestHitPose, nearestHit, true);
        }
    }
    
    //////////////////////////////// Spawn fortress methods /////////////////////////////
    private void CheckSpawnFortress(Vector2 screenPosition, ARRaycastHit nearestHitPose, RaycastHit nearestRayHit, bool isArPlaneManagerEnabled)
    {
        if (cooldownCount > cooldown) 
        {
            Debug.Log("checking Spawn");
            if (isArPlaneManagerEnabled)
            {
                doSpawnFortressAR(screenPosition, nearestHitPose);
            }
            else {
                doSpawnFortressGroundPlane(screenPosition, nearestRayHit);
            }
            cooldownCount = 0;
        }
    }

    public void doSpawnFortressGroundPlane(Vector2 screenPosition, RaycastHit nearestRayHit)
    {
        spawnedFortress = Instantiate(fortressPrefab, nearestRayHit.point, Quaternion.identity);

        SetObjectIsKinematic(spawnedFortress, true);
        // Debug.Log("spawning on Ground Plane");

        spawnedFortress.transform.parent = setup.content.transform;
        logger.Log("spawnedfortress parent:  " + spawnedFortress.transform.parent.name);

        // arSessionOrigin.MakeContentAppearAt(content.transform, groundPlane.transform.position);
        arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, nearestRayHit.point);
    }

    public void doSpawnFortressAR(Vector2 screenPosition, ARRaycastHit nearestHitPose) {

        ARPlane plane;
        ARAnchor point;

        spawnedFortress = Instantiate(fortressPrefab, nearestHitPose.pose.position 
            + nearestHitPose.pose.up * 0.05f, nearestHitPose.pose.rotation);

        spawnedFortress.transform.parent = setup.content.transform;
        logger.Log("spawnedfortress parent:  " + spawnedFortress.transform.parent.name);
        
        SetObjectIsKinematic(spawnedFortress, true);
        arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, nearestHitPose.pose.position);

        plane = planeManager.GetPlane(nearestHitPose.trackableId);

        if (plane != null) {
            point = anc.AttachAnchor(plane, nearestHitPose.pose);
            logger.Log("Added an anchor to a plane " + nearestHitPose);
        } else {
            point = spawnedFortress.AddComponent<ARAnchor>();
            logger.Log("Added another anchor " + nearestHitPose);
        }
        
        spawnedFortress.transform.parent = point.transform;
    }

    /////////////////////////////////////////////////////////////
    private void CheckSelect(bool rayHit, bool ARhit, RaycastHit nearestHit, ARRaycastHit nearestHitPose, Vector2 screenPosition)
    {
        if (rayHit) {
            logger.Log ("Detected " + nearestHit.transform.gameObject.name);
            
            if (nearestHit.transform.gameObject.layer == LayerManager.BlockLayer || 
                nearestHit.transform.gameObject.layer == LayerManager.GroundLayer) {
                foundObject = nearestHit.transform.gameObject;
                foundObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }
    private void CheckMove(bool rayHit, bool ARhit, RaycastHit nearestHit, ARRaycastHit nearestHitPose, Vector2 screenPosition)
    {
        if (planeManager.enabled == false || planeManager.requestedDetectionMode == PlaneDetectionMode.None)
        {
            // logger.Log ("plane manager disabled");
            if (rayHit)
            {
                if (nearestHit.transform.gameObject.layer == LayerManager.GroundLayer) {
                    foundObject.transform.position = nearestHit.point;
                    return;
                }
            }
        }
        else if (ARhit == true)
        {
            foundObject.transform.position = nearestHitPose.pose.position;
        }
    }

    public void ConfirmPlacement()
    {
        if (spawnedFortress == null)
        {
            Debug.Log("Must place fortress before moving on");
            return;
        }

        if (GameManager.Instance.currentPlayer == CurrentPlayer.PLAYER_1)
        {
            GameManager.Instance.currentPlayer = CurrentPlayer.PLAYER_2;
            p1SpawnedFortress = spawnedFortress; // remember p1 core block prefab
            spawnedFortress = null;
            UpdateUI();
        }
        // must be player 2
        // set both to non-kinematic, return to p1
        else {
            GameManager.Instance.SetGameState(GameState.PLACE_CORE_BLOCK);
            ActivatePhysics();
            GameManager.Instance.currentPlayer = CurrentPlayer.PLAYER_1;
            BackToMainCanvas();
        }

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ActivatePhysics()
    {
        if (spawnedFortress == null) { return; }
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == LayerManager.BlockLayer)
            {
                SetObjectIsKinematic(goArray[i], false);
                SetObjectGravity(goArray[i], true);
            }
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

    public void SetObjectGravity(GameObject spawnedObject, bool onOrOff)
    {
        Rigidbody[] rbs = spawnedObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = onOrOff;
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ChangeToSelect()
    {   
        placeMode = PlaceMode.SELECT;   
        sessionController.UpdateText();
    }
    public void ChangeToPlace()
    {   
        placeMode = PlaceMode.PLACE;
        sessionController.UpdateText();   
    }
    public void ChangeToMove()
    {   
        placeMode = PlaceMode.MOVE;
        sessionController.UpdateText();   
    }
    public void ChangeToFire()
    {   
        placeMode = PlaceMode.FIRE;
        sessionController.UpdateText();   
    }















}
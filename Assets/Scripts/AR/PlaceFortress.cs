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
    MOVE
}

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceFortress: MonoBehaviour {

    private InputManager inputManager;
    [SerializeField] private PlaceMode placeMode;

    public TMP_Text currentModeText;
    public TMP_Text currentFortSize;

    public GameObject fortressPrefab;
    private ARRaycastManager raycastManager;
    private Vector2 touchPosition;

    static List<ARRaycastHit> myARHits = new List <ARRaycastHit>();
    private GameObject foundObject = null;

    public Camera myCamera;
    public float cooldown, cooldownCount;
    private ARAnchorManager anc;
    private ARPlaneManager planeManager;

    private ARSessionOrigin arSessionOrigin;
    private float arSessionOriginSize = 150;
    private float upscaleIncrement = 0.9f;
    private float downscaleIncrement = 1.1f;

    public GameObject refPlane;
    public GameObject groundPlane;
    public GameObject spawnedFortress;
    public bool enableAppear = true;

    private static ILogger logger = Debug.unityLogger;

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
        
        inputManager = InputManager.Instance;   
        inputManager.OnFirstTouch += CheckTouchAction;
        UpdateText();
        // ScaleArOrigin();
    }

    void Update() {
        if (cooldownCount <= 2)
        {
            cooldownCount += Time.deltaTime;
        }
        
        if (enableAppear && refPlane != null)
        {
            Vector3 targetPos = new Vector3(spawnedFortress.transform.position.x, refPlane.transform.position.y, spawnedFortress.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, targetPos);
        }

        // if (groundPlane != null)
        // {
        //     if (planeManager.requestedDetectionMode != PlaneDetectionMode.None)
        //     {
        //         Debug.Log("Ground plane found: " + groundPlane);
        //         planeManager.requestedDetectionMode = PlaneDetectionMode.None;
        //     }
        // }
            // Debug.Log("making content appear");
    }

    private void CheckSpawnFortress(Vector2 screenPosition, ARRaycastHit nearestHitPose, RaycastHit nearestRayHit, bool isArPlaneManagerEnabled)
    {
        if (cooldownCount > cooldown) {
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
        spawnedFortress = Instantiate(fortressPrefab, nearestRayHit.transform.position 
            + nearestRayHit.transform.up * 0.05f, nearestRayHit.transform.rotation);

        SetObjectIsKinematic(spawnedFortress, true);
        Debug.Log("spawning on Ground Plane");

        arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, nearestRayHit.transform.position);
    }

    public void doSpawnFortressAR(Vector2 screenPosition, ARRaycastHit nearestHitPose) {

        ARPlane plane;
        ARAnchor point;

        spawnedFortress = Instantiate(fortressPrefab, nearestHitPose.pose.position 
            + nearestHitPose.pose.up * 0.05f, nearestHitPose.pose.rotation);

        SetObjectIsKinematic(spawnedFortress, true);
        arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, nearestHitPose.pose.position);

        // spawnedFortress.transform.localScale = fortressSize;

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

    private void CheckTouchAction(Touch touch)
    {
        bool ARhit;
        ARRaycastHit nearestHitPose = new ARRaycastHit();

        RaycastHit[] hits;
        Ray ray;

        Vector2 screenPosition = touch.position;
        // Debug.Log ("position:  " + screenPosition);

        if (IsPointOverUIObject(screenPosition))
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

        ray = myCamera.ScreenPointToRay(screenPosition);
        hits = Physics.RaycastAll(ray);
        
        switch (placeMode)
        {
            case PlaceMode.PLACE:
                if (planeManager.enabled == false || planeManager.requestedDetectionMode == PlaneDetectionMode.None)
                {
                    logger.Log ("plane manager disabled");

                    foreach (RaycastHit hit in hits) {
                        if (hit.transform.gameObject.layer == LayerManager.GroundLayer) {
                            // foundObject = hit.transform.gameObject;
                            logger.Log ("hit ground");
                            CheckSpawnFortress(screenPosition, nearestHitPose, hit, false);
                            break;
                        }
                    }
                }

                else if (ARhit == true)
                {
                    CheckSpawnFortress(screenPosition, nearestHitPose, hits[0], true);
                }

                foundObject = null;
                break;
            
            case PlaceMode.SELECT:

                foreach (RaycastHit hit in hits) {
                    logger.Log ("Detected " + hit.transform.gameObject.name);
                    
                    if (hit.transform.gameObject.layer == LayerManager.BlockLayer) {
                        foundObject = hit.transform.gameObject;
                        logger.Log ("found block: " + foundObject);
                    }
                }
                break;
            
            case PlaceMode.MOVE:
                if (foundObject == null)
                {
                    Debug.Log ("Nothing Selected");
                }
                else {
                    if (ARhit == true)
                    {
                        foundObject.transform.position = nearestHitPose.pose.position;
                    }
                }
                break;
            }
        // }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

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

    public void ActivateKinematic()
    {
        if (spawnedFortress == null) { return; }
        SetObjectIsKinematic(spawnedFortress, false);
    }

    public void DisableGravity()
    {
        if (spawnedFortress == null) { return; }
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == LayerManager.BlockLayer)
            {
                SetObjectGravity(goArray[i], false);
            }
        }
    }
    public void ActivateGravity()
    {
        if (spawnedFortress == null) { return; }
        SetObjectGravity(spawnedFortress, true);
    }

    public void ChangeToSelect()
    {   
        placeMode = PlaceMode.SELECT;   
        // Debug.Log ("Changing to Select");
        UpdateText();
    }
    public void ChangeToPlace()
    {   
        placeMode = PlaceMode.PLACE;
        UpdateText();   
    }
    public void ChangeToMove()
    {   
        placeMode = PlaceMode.MOVE;
        UpdateText();   
    }

    public void UpscaleSession()
    {   
        // arSessionOriginSize += sizeIncrement;
        ScaleArOrigin(upscaleIncrement);
        UpdateText();  
    }
    public void DownscaleSession()
    {   
        // arSessionOriginSize -= sizeIncrement; 
        ScaleArOrigin(downscaleIncrement);
        UpdateText();
    }

    private void ScaleArOrigin(float multiplier)
    {
        arSessionOriginSize = arSessionOriginSize * multiplier;
        arSessionOrigin.transform.localScale = Vector3.one * arSessionOriginSize;
        if (groundPlane != null)
        {
            Vector3 targetPos = new Vector3(spawnedFortress.transform.position.x, groundPlane.transform.position.y, spawnedFortress.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, groundPlane.transform.position);
            return;
        }
        arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, spawnedFortress.transform.position);
    }

    private void UpdateText()
    {
        currentModeText.text = placeMode.ToString();
        currentFortSize.text = arSessionOrigin.transform.localScale.ToString("F3");
    }

    public bool IsPointOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject()) 
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.currentSelectedGameObject.layer == LayerManager.UILayer)
                {
                    return true;
                }
            }
            return false;
        }

       PointerEventData eventPosition = new PointerEventData(EventSystem.current);
       eventPosition.position = pos;

       List<RaycastResult> results = new List<RaycastResult>();
       EventSystem.current.RaycastAll(eventPosition, results);

       return results.Count > 0;

    }



}
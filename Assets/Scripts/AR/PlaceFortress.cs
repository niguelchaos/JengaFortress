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

    private InputManager inputManager;
    [SerializeField] private PlaceMode placeMode;
    private ScaleContent scaleContent;
    private SessionOriginController sessionController;
    private ImageTracker imageTracker;


    public GameObject fortressPrefab;
    private ARRaycastManager raycastManager;
    private Vector2 touchPosition;

    static List<ARRaycastHit> myARHits = new List <ARRaycastHit>();
    private GameObject foundObject = null;
    public GameObject spawnGroundReticle;

    public Camera myCamera;
    public float cooldown, cooldownCount;
    private ARAnchorManager anc;
    public ARPlaneManager planeManager;
    private ARSessionOrigin arSessionOrigin;

    public GameObject groundPlane {get; set;}
    public GameObject refPlane;
    public GameObject spawnedFortress;
    public GameObject content;


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
        scaleContent = this.gameObject.GetComponent<ScaleContent>();
        sessionController = this.gameObject.GetComponent<SessionOriginController>();
        imageTracker = this.gameObject.GetComponent<ImageTracker>();
        
        inputManager = InputManager.Instance;   
        inputManager.OnFirstTouch += CheckTouchAction;
        // sessionController.UpdateText();

        if (refPlane.activeSelf == true && planeManager.enabled)
        {
            content.transform.position = refPlane.transform.position;
            // refPlane.transform.parent = arSessionOrigin.transform;
        }

    }

    void Update() {
        if (cooldownCount <= 2)
        {
            cooldownCount += Time.deltaTime;
        }
        
        if (refPlane.activeSelf == true && planeManager.enabled)
        {
            // Vector3 targetPos = new Vector3(spawnedFortress.transform.position.x, refPlane.transform.position.y, spawnedFortress.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(content.transform, refPlane.transform.position);
        }
    }

   

    private void CheckTouchAction(Touch touch)
    {
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
                    Debug.Log ("Nothing Selected");
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

    private void CheckPlace(bool rayHit, bool ARhit, RaycastHit nearestHit, ARRaycastHit nearestHitPose, Vector2 screenPosition)
    {
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

        else if (ARhit == true)
        {
            CheckSpawnFortress(screenPosition, nearestHitPose, nearestHit, true);
        }
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
        spawnedFortress = Instantiate(fortressPrefab, nearestRayHit.point, Quaternion.identity);

        SetObjectIsKinematic(spawnedFortress, true);
        // Debug.Log("spawning on Ground Plane");

        spawnedFortress.transform.parent = content.transform;
        logger.Log("spawnedfortress parent:  " + spawnedFortress.transform.parent.name);

        // arSessionOrigin.MakeContentAppearAt(content.transform, groundPlane.transform.position);
        arSessionOrigin.MakeContentAppearAt(spawnedFortress.transform, nearestRayHit.point);
    }

    public void doSpawnFortressAR(Vector2 screenPosition, ARRaycastHit nearestHitPose) {

        ARPlane plane;
        ARAnchor point;

        spawnedFortress = Instantiate(fortressPrefab, nearestHitPose.pose.position 
            + nearestHitPose.pose.up * 0.05f, nearestHitPose.pose.rotation);

        spawnedFortress.transform.parent = content.transform;
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

        if (ARhit)
        {
            spawnGroundReticle.transform.position = nearestHitPose.pose.position;
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
    public void EnableRefPlane()
    {   
        bool active = !refPlane.gameObject.activeSelf; 
        refPlane.gameObject.SetActive(active);
        sessionController.UpdateText();   
    }

    public void PlaceGroundPlaneOnReticle()
    {
        imageTracker.PlaceGroundPlane(spawnGroundReticle.transform.position);
    }












}
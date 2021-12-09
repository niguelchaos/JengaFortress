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

    private Vector3 fortressSize = new Vector3(0.02f, 0.02f, 0.02f);
    private Vector3 sizeIncrement = new Vector3(0.005f, 0.005f, 0.005f);

    private static ILogger logger = Debug.unityLogger;


    private void Awake()
    {
        inputManager = InputManager.Instance;
        
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += CheckTapAction;
    }
    private void OnDisable()
    {
        inputManager.OnStartTouch -= CheckTapAction;
    }

    void Start() {
        cooldown = 2;
        myCamera = 
        	this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();

        raycastManager = this.gameObject.GetComponent<ARRaycastManager>();
        anc = this.gameObject.GetComponent<ARAnchorManager>();
        planeManager = this.gameObject.GetComponent<ARPlaneManager>();
        UpdateText();
    }

    void Update() {
        cooldownCount += Time.deltaTime;
    }

    private void CheckSpawnFortress(Vector2 screenPosition, bool ARhit, ARRaycastHit nearestHitPose)
    {
        if (cooldownCount > cooldown && ARhit == true) {
            Debug.Log("checking Spawn");
            doSpawnFortress(screenPosition, ARhit, nearestHitPose);
            cooldownCount = 0;
        }
    }

    public void doSpawnFortress(Vector2 screenPosition, bool ARhit, ARRaycastHit nearestHitPose) {
        GameObject spawnedFortress;

        ARPlane plane;
        ARAnchor point;


        spawnedFortress = Instantiate(fortressPrefab, nearestHitPose.pose.position 
            + nearestHitPose.pose.up * 0.1f, nearestHitPose.pose.rotation);

        SetObjectIsKinematic(spawnedFortress, true);

        spawnedFortress.transform.localScale = fortressSize;
        // spawnedFortress.tag = "SpawnedObject";

        logger.Log("spawned at " + spawnedFortress.transform.position.x + ", " 
        + spawnedFortress.transform.position.y + ", " + spawnedFortress.transform.position.z);

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

    private void CheckTapAction(Vector2 screenPosition, float time)
    {
        bool ARhit;
        ARRaycastHit nearestHitPose = new ARRaycastHit();

        RaycastHit[] hits;
        Ray ray;


        if (EventSystem.current.IsPointerOverGameObject())  {
            if (EventSystem.current.currentSelectedGameObject.layer == LayerManager.UILayer)
            {
                logger.Log ("clicked on button");
                return;
            }
        }
        /////////////////////////////////////////////////////////////////////

        ARhit = raycastManager.Raycast(screenPosition, myARHits,
            TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon);

        logger.Log("Hit: " + ARhit);

        if (ARhit == true) {
            nearestHitPose = myARHits[0];
        }
        
        switch (placeMode)
        {
            case PlaceMode.PLACE:
                CheckSpawnFortress(screenPosition, ARhit, nearestHitPose);
                foundObject = null;
                break;
            
            case PlaceMode.SELECT:
                ray = myCamera.ScreenPointToRay(screenPosition);
                hits = Physics.RaycastAll(ray);

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
    }

    private void SetObjectIsKinematic(GameObject spawnedObject, bool isKOrNotIdkMan)
    {
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = isKOrNotIdkMan;
        }
    }

    public void ActivatePhysics()
    {
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == LayerManager.BlockLayer)
            {
                SetObjectIsKinematic(goArray[i], false);
            }
        }
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

    public void UpscaleFortress()
    {   
        fortressSize += sizeIncrement;
        UpdateText();  
    }
    public void DownscaleFortress()
    {   
        fortressSize -= sizeIncrement; 
        UpdateText();
    }

    private void UpdateText()
    {
        currentModeText.text = placeMode.ToString();
        currentFortSize.text = fortressSize.ToString("F3");
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceFortress: MonoBehaviour {

    private InputManager inputManager;

    public GameObject fortressPrefab;
    private ARRaycastManager raycastManager;
    private Vector2 touchPosition;

    static List<ARRaycastHit> myARHits = new List <ARRaycastHit>();

    public Camera myCamera;
    public float cooldown, cooldownCount;
    private ARAnchorManager anc;
    private ARPlaneManager planeManager;

    private static ILogger logger = Debug.unityLogger;


    private void Awake()
    {
        inputManager = InputManager.Instance;
        
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += CheckSpawnFortress;
    }
    private void OnDisable()
    {
        inputManager.OnStartTouch -= CheckSpawnFortress;
    }

    void Start() {
        cooldown = 2;
        myCamera = 
        	this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();

        raycastManager = this.gameObject.GetComponent<ARRaycastManager>();
        anc = this.gameObject.GetComponent<ARAnchorManager>();
        planeManager = this.gameObject.GetComponent<ARPlaneManager>();

    }

    void Update() {
        cooldownCount += Time.deltaTime;
    }

    private void CheckSpawnFortress(Vector2 screenPosition, float time)
    {
        if (cooldownCount > cooldown) {
            Debug.Log("checking Spawn");
            doSpawnFortress(screenPosition, time);
            cooldownCount = 0;
        }
    }



    public void doSpawnFortress(Vector2 screenPosition, float time) {
        GameObject spawnedFortress;
        GameObject foundObject = null;
        // Vector3 screenCenter;
        bool ARhit;
        ARRaycastHit nearestHitPose;
        // List<ARRaycastHit> myHits = new List <ARRaycastHit>();
        ARPlane plane;
        ARAnchor point;

        RaycastHit[] hits;
        Ray ray;

        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, myCamera.nearClipPlane);
        Vector3 worldCoords = myCamera.ScreenToWorldPoint(screenCoordinates);
        // screenCenter = myCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        ARhit = raycastManager.Raycast(screenPosition, myARHits,
            TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon);

        logger.Log("Hit: " + ARhit);

        
        if (ARhit == true) {
            nearestHitPose = myARHits[0];
            
            ray = myCamera.ScreenPointToRay(screenPosition);
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits) {
			    logger.Log ("Detected " + hit.transform.gameObject.name);
				
                if (hit.transform.gameObject.tag == "SpawnedObject") {
                    logger.Log ("found spawned object");
                    foundObject = hit.transform.gameObject;
                }
            }

            if (foundObject == null)
            {
                spawnedFortress = Instantiate(fortressPrefab, nearestHitPose.pose.position 
                    + nearestHitPose.pose.up * 0.1f, nearestHitPose.pose.rotation);

                // spawnedFortress.transform.localScale = new Vector3(3, 3, 3);
                spawnedFortress.tag = "SpawnedObject";

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
            else {
                foundObject.transform.position = nearestHitPose.pose.position;
            }
            

        }
    }

}
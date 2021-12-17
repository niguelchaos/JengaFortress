using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager imgtracker;
    private ARSessionOrigin arOrigin;
    private ARAnchorManager arAnchor;
    private ARPlaneManager planeManager;
    // public GameObject fortress;
    public PlaceFortress placeFortress;
    public GameObject groundPlanePrefab;
    public Dictionary <string, GameObject> myGroundPlanes;

    void Awake()
    {
        imgtracker = GetComponent<ARTrackedImageManager>();
        myGroundPlanes = new Dictionary<string, GameObject>();
        arOrigin = GetComponent<ARSessionOrigin>();
        arAnchor = GetComponent<ARAnchorManager>();
    }

    private void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    void OnEnable()
    {
        imgtracker.trackedImagesChanged += handleTrackingEvent;
    }

    void OnDisable()
    {
        imgtracker.trackedImagesChanged -= handleTrackingEvent;
    }


  void handleTrackingEvent(ARTrackedImagesChangedEventArgs eventArgs)
    {
      
        foreach (ARTrackedImage img in eventArgs.added)
        {
            Debug.Log ("handling image");
            handleTracking(img);
        }

        foreach (ARTrackedImage img in eventArgs.updated)
        {
            // Debug.Log ("updating image");
            handleTracking(img);
        }
    }

  void handleTracking (ARTrackedImage img)
    {
        // GameObject currentFortress;
        string key;

        if (img.trackingState == TrackingState.None) {
            // Debug.Log ("not tracking image");
            return;
        }

        // Debug.Log ("Found an image: " + img.referenceImage.name + " (" 
        //    + img.trackingState + ")");

        key = img.referenceImage.name;

        img.transform.Translate (0.1f, 0, 0);


        if (!myGroundPlanes.ContainsKey(key)) {
            // currentFortress = Instantiate (fortress, img.transform.position, img.transform.rotation);
            // fortress.transform.localScale = new Vector3(1, 1, 1);
            // fortress.transform.parent = img.transform;
            // myFortresses[key] = fortress;
            if (placeFortress.groundPlane == null)
            {
                Debug.Log ("image pos:  " + img.transform.position);
                placeFortress.groundPlane = Instantiate(groundPlanePrefab, img.transform.position, Quaternion.identity);
                placeFortress.transform.parent = img.transform;
                myGroundPlanes[key] = groundPlanePrefab;

                placeFortress.groundPlane.AddComponent<ARAnchor>();
                // placeFortress.groundPlane.transform.parent = arOrigin.transform;

                placeFortress.content.transform.position = placeFortress.groundPlane.transform.position;
                Debug.Log ("groundplane pos:  " + placeFortress.groundPlane.transform.position);
                
                
                planeManager.requestedDetectionMode = PlaneDetectionMode.None;
                planeManager.enabled = false;
                Debug.Log ("planes disabled");
            }
        }
    }
}

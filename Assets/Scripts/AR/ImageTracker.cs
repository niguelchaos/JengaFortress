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
    private PlaceFortress placeFortress;
    private ScaleContent scaleContent;
    public GameObject groundPlanePrefab;
    public Dictionary <string, GameObject> myGroundPlanes;
    public GameObject testLocation;

    void Awake()
    {
        imgtracker = GetComponent<ARTrackedImageManager>();
        myGroundPlanes = new Dictionary<string, GameObject>();
        arOrigin = GetComponent<ARSessionOrigin>();
        arAnchor = GetComponent<ARAnchorManager>();
        placeFortress = GetComponent<PlaceFortress>();
        scaleContent = GetComponent<ScaleContent>();
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
            if (placeFortress.groundPlane == null)
            {
                Debug.Log ("image pos:  " + img.transform.position);
                placeFortress.groundPlane = Instantiate(groundPlanePrefab, img.transform.position, Quaternion.identity);
                // placeFortress.transform.parent = img.transform;
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

    public void PlaceGroundPlane()
    {
        if (testLocation.activeSelf == true)
        {
            Debug.Log ("image pos:  " + testLocation.transform.position);
            placeFortress.groundPlane = Instantiate(groundPlanePrefab, testLocation.transform.position, Quaternion.identity);
            // placeFortress.transform.parent = testLocation.transform;
            // myGroundPlanes[key] = groundPlanePrefab;

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

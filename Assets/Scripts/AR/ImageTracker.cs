using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager imgtracker;
    private ARPlaneManager planeManager;
    public GameObject fortress;
    public PlaceFortress placeFortress;
    public GameObject groundPlanePrefab;
    public Dictionary <string, GameObject> myFortresses;

    void Awake()
    {
        imgtracker = GetComponent<ARTrackedImageManager>();
        myFortresses = new Dictionary<string, GameObject>();
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

        if (!myFortresses.ContainsKey(key)) {
            // currentFortress = Instantiate (fortress, img.transform.position, img.transform.rotation);
            // fortress.transform.localScale = new Vector3(1, 1, 1);
            // fortress.transform.parent = img.transform;
            // myFortresses[key] = fortress;
            if (placeFortress.groundPlane == null)
            {
                placeFortress.groundPlane = Instantiate(groundPlanePrefab, img.transform.position, Quaternion.identity);
                planeManager.requestedDetectionMode = PlaneDetectionMode.None;
                planeManager.enabled = false;
                Debug.Log ("planes disabled");
            }
        }
    }
}

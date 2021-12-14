using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RobotBehaviour: MonoBehaviour {
    private ARRaycastManager rays;
    public GameObject robotPrefab;
    public Camera myCamera;
    public float cooldown, cooldownCount;
    private ARAnchorManager anc;
    private ARPlaneManager plan;

    private static ILogger logger = Debug.unityLogger;

    void Start() {
        cooldown = 2;
        myCamera = 
        	this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();

        rays = this.gameObject.GetComponent<ARRaycastManager>();
        anc = this.gameObject.GetComponent<ARAnchorManager>();
        plan = this.gameObject.GetComponent<ARPlaneManager>();

    }

    void Update() {

        
        cooldownCount += Time.deltaTime;

        if(Input.touchCount == 2) {
            logger.Log("touched 2");
        }

        if(Input.touchCount == 1) {
            logger.Log("touched 1");
        }
        
        if (cooldownCount > cooldown && Input.touchCount == 2) {
            doSpawnRobot();
            cooldownCount = 0;
            logger.Log("Tapped screen");
        }


    }

    public void doSpawnRobot() {
        GameObject robot;
        Vector3 screenCenter;
        bool hit;
        ARRaycastHit nearest;
        List<ARRaycastHit> myHits = new List <ARRaycastHit>();
        ARPlane plane;
        ARAnchor point;

        screenCenter = myCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        hit = rays.Raycast(screenCenter,
            myHits,
            TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon);

        logger.Log("Hit: " + hit);

        if (hit == true) {
            nearest = myHits[0];
            robot = Instantiate(robotPrefab, nearest.pose.position 
                + nearest.pose.up * 0.1f, nearest.pose.rotation);

            robot.transform.localScale = new Vector3(3, 3, 3);
            robot.tag = "SpawnedObject";

            logger.Log("spawned at " + robot.transform.position.x + ", " 
               + robot.transform.position.y + ", " + robot.transform.position.z);

            plane = plan.GetPlane(nearest.trackableId);

            if (plane != null) {
                point = anc.AttachAnchor(plane, nearest.pose);
                logger.Log("Added an anchor to a plane " + nearest);
            } else {
                point = anc.AddAnchor(nearest.pose);
                logger.Log("Added another anchor " + nearest);

            }

            robot.transform.parent = point.transform;

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

// this entire class is jank
public class ScaleContent: MonoBehaviour 
{
    private PlaceFortress placeFortress;
    private ARFireProjectile fireProjectile;
    private Setup setup;
    private ARSessionOrigin arSessionOrigin;
    private SessionOriginController sessionController;

    public Camera myCamera;

    public float arSessionOriginSize = 55;
    private float upscaleIncrement = 5f;
    private float downscaleIncrement = -5f;

    // [SerializeField] public Vector3 selectedFirePosition;
    [SerializeField] public Vector3 prevFirePosition;
    // public GameObject firingPos;
    public bool prevSpawnDistActivated = false;

    // public Slider firingPosSlider;

    private void Awake()
    {
        arSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    void Start() 
    {
        setup = GetComponent<Setup>();
        placeFortress = GetComponent<PlaceFortress>();
        fireProjectile = GetComponent<ARFireProjectile>();
        arSessionOrigin = GetComponent<ARSessionOrigin>();
        sessionController = this.gameObject.GetComponent<SessionOriginController>();

        myCamera = this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();
    }

    ////////////////////////////////////////////////////////////////
    public void UpscaleSession()
    {   
        ScaleArOrigin(upscaleIncrement);
        sessionController.UpdateText();  
    }
    public void DownscaleSession()
    {   
        ScaleArOrigin(downscaleIncrement);
        sessionController.UpdateText();
    }

    public void ScaleArOrigin(float increment)
    {
        arSessionOriginSize = arSessionOriginSize + increment;
        arSessionOrigin.transform.localScale = Vector3.one * arSessionOriginSize;

        // since camera has moved, need correct previous position for accurate previous distance
        // doesnt work in the beginning if previous distance is 1
        if (prevSpawnDistActivated)
        {
            setup.firingPos.transform.position = myCamera.gameObject.transform.position + (transform.forward * fireProjectile.prevFireSpawnDist);
        }
        
        // remember this previous firepos
        // prevFirePosition = firingPos.transform.position;
        // fireProjectile.prevFireSpawnDist = fireProjectile.fireSpawnDist;
        // Debug.Log ("firedist: ");        
        PlaceCameraOnContent();
        // arSessionOrigin.MakeContentAppearAt(content.transform, content.transform.position);
        // CalcFireDist(increment);
        // Debug.Log ("current: " + fireProjectile.fireSpawnDist);
        prevSpawnDistActivated = true;
    }

    private void PlaceCameraOnContent()
    {
        if (placeFortress.refPlane.activeSelf == true && placeFortress.planeManager.enabled)
        {
            arSessionOrigin.MakeContentAppearAt(placeFortress.content.transform, placeFortress.refPlane.transform.position);
        }

        else if (setup.groundPlane != null)
        {
            // Vector3 targetPos = new Vector3(content.transform.position.x, groundPlane.transform.position.y, content.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(placeFortress.content.transform, setup.groundPlane.transform.position);
            Debug.Log ("ground plane scaling");
            // return;
        }
    }

    private void CalcFireDist(float increment)
    {
        float fireDistDiff;

        // use unmodified position to get difference between previous and current
        Vector3 currentPos = myCamera.gameObject.transform.position + (transform.forward * fireProjectile.fireSpawnDist);
        
        fireDistDiff = GetFireSpawnDistDiff(currentPos);
        Debug.Log("Firing Pos in Place, diff:  " + fireDistDiff);

        // add or sub this distance difference according to scale
        // only modify spawndist if scale has changed
        if (increment < 0)
        {
            fireProjectile.fireSpawnDist -= fireDistDiff; 
        }
        else if (increment > 0)
        {
            fireProjectile.fireSpawnDist += fireDistDiff; 
        }

        // apply distance difference
        setup.firingPos.transform.position = myCamera.gameObject.transform.position + (transform.forward * fireProjectile.fireSpawnDist);
    }

    private float GetFireSpawnDistDiff(Vector3 currentPos)
    {
        float distance = Vector3.Distance(prevFirePosition, currentPos);
        return distance;
    }


    

    
}
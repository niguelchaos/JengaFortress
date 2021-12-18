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
    private ARSessionOrigin arSessionOrigin;
    public Camera myCamera;

    public float arSessionOriginSize = 55;
    private float upscaleIncrement = 5f;
    private float downscaleIncrement = -5f;

    [SerializeField] public Vector3 selectedFirePosition;
    [SerializeField] public Vector3 prevFirePosition;
    public GameObject firingPos;
    public bool prevSpawnDistActivated = false;

    public Slider firingPosSlider;


    void Start() 
    {
        placeFortress = GetComponent<PlaceFortress>();
        fireProjectile = GetComponent<ARFireProjectile>();
        arSessionOrigin = GetComponent<ARSessionOrigin>();
        myCamera = this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();


        // GetInitFirePosition();
    }



    public void SetFiringPos()
    {
        firingPos.transform.position = (myCamera.transform.forward * firingPosSlider.value);
    }

    public void GetInitFirePosition()
    {
        // PlaceCameraOnContent();
        // Jank way to get distance
        // arSessionOrigin.transform.localScale = Vector3.one;
        // // remember session pos
        // myCamera.gameObject.transform.position + 
  				// (myCamera.gameObject.transform.forward * fireSpawnDist)
        selectedFirePosition = firingPos.transform.position;
        Debug.Log("Remembering inital position:  " + selectedFirePosition);

        // // return to original scale
        // ScaleArOrigin(0);

        fireProjectile.fireSpawnDist = GetInitFireSpawnDist();
        // firingPos.transform.position = selectedFirePosition + (transform.forward * fireProjectile.fireSpawnDist);
        // Debug.Log("Firing Pos in Place: ");

        // UpscaleSession();
    }

    public float GetInitFireSpawnDist()
    {
        // arsession gameobject has moved far away at this point
        float distance = Vector3.Distance(selectedFirePosition, myCamera.gameObject.transform.position);
        Debug.Log("init spawn dist: " + distance);
        return distance;
    }



    public void UpscaleSession()
    {   
        // arSessionOriginSize += sizeIncrement;
        ScaleArOrigin(upscaleIncrement);
        placeFortress.UpdateText();  
    }
    public void DownscaleSession()
    {   
        // arSessionOriginSize -= sizeIncrement; 
        ScaleArOrigin(downscaleIncrement);
        placeFortress.UpdateText();
    }

    public void ScaleArOrigin(float increment)
    {
        arSessionOriginSize = arSessionOriginSize + increment;
        arSessionOrigin.transform.localScale = Vector3.one * arSessionOriginSize;

        // since camera has moved, need correct previous position for accurate previous distance
        // doesnt work in the beginning if previous distance is 1
        if (prevSpawnDistActivated)
        {
            // firingPos.transform.position = myCamera.gameObject.transform.position + (transform.forward * fireProjectile.prevFireSpawnDist);
        }
        
        // remember this previous firepos
        // prevFirePosition = firingPos.transform.position;
        // fireProjectile.prevFireSpawnDist = fireProjectile.fireSpawnDist;
        // Debug.Log ("firedist: ");        
        PlaceCameraOnContent();
        // arSessionOrigin.MakeContentAppearAt(content.transform, content.transform.position);
        CalcFireDist(increment);
        Debug.Log ("current: " + fireProjectile.fireSpawnDist);
        prevSpawnDistActivated = true;
    }

    private void PlaceCameraOnContent()
    {
        if (placeFortress.refPlane != null)
        {
            arSessionOrigin.MakeContentAppearAt(placeFortress.content.transform, placeFortress.refPlane.transform.position);
        }

        if (placeFortress.groundPlane != null)
        {
            // Vector3 targetPos = new Vector3(content.transform.position.x, groundPlane.transform.position.y, content.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(placeFortress.content.transform, placeFortress.groundPlane.transform.position);
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
        firingPos.transform.position = myCamera.gameObject.transform.position + (transform.forward * fireProjectile.fireSpawnDist);
    }

    private float GetFireSpawnDistDiff(Vector3 currentPos)
    {
        float distance = Vector3.Distance(prevFirePosition, currentPos);
        // Debug.Log("dist diff: " + distance);
        return distance;
    }


    

    
}
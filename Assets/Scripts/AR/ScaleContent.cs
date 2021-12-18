using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ScaleContent: MonoBehaviour 
{
    private PlaceFortress placeFortress;
    private ARFireProjectile fireProjectile;
    private ARSessionOrigin arSessionOrigin;

    public float arSessionOriginSize = 55;
    private float upscaleIncrement = 5f;
    private float downscaleIncrement = -5f;

    [SerializeField] public Vector3 initFirePosition;
    [SerializeField] public Vector3 currentFirePosition;
    public GameObject firingPos;


    void Start() 
    {
        placeFortress = GetComponent<PlaceFortress>();
        fireProjectile = GetComponent<ARFireProjectile>();
        arSessionOrigin = GetComponent<ARSessionOrigin>();

        GetInitFirePosition();
    }

    private void GetInitFirePosition()
    {
        // Jank way to get distance
        arSessionOrigin.transform.localScale = Vector3.one;
        // remember session pos
        initFirePosition = arSessionOrigin.transform.position;
        Debug.Log("ehh:  " + initFirePosition);

        // return to original scale
        ScaleArOrigin(0);

        fireProjectile.fireSpawnDist = GetInitFireSpawnDist();
        firingPos.transform.position = initFirePosition + (transform.forward * fireProjectile.fireSpawnDist);
    }

    public float GetInitFireSpawnDist()
    {
        // arsession gameobject has moved far away at this point
        float distance = Vector3.Distance(initFirePosition, this.transform.position);
        Debug.Log("dist: " + distance);
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

    public void ScaleGroundPlane()
    {
        
    }

    public void ScaleArOrigin(float increment)
    {
        // arSessionOriginSize = arSessionOriginSize * multiplier;
        fireProjectile.prevFireSpawnDist = fireProjectile.fireSpawnDist;
        Debug.Log ("prev: " + fireProjectile.prevFireSpawnDist );

        arSessionOriginSize = arSessionOriginSize + increment;
        arSessionOrigin.transform.localScale = Vector3.one * arSessionOriginSize;

        // Debug.Log ("firedist: ");

        
        if (placeFortress.refPlane != null)
        {
            // Vector3 targetPos = new Vector3(spawnedFortress.transform.position.x, refPlane.transform.position.y, spawnedFortress.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(placeFortress.content.transform, placeFortress.refPlane.transform.position);
        }

        if (placeFortress.groundPlane != null)
        {
            // Vector3 targetPos = new Vector3(content.transform.position.x, groundPlane.transform.position.y, content.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(placeFortress.content.transform, placeFortress.groundPlane.transform.position);
            Debug.Log ("ground plane scaling");
            // return;
        }
        // Debug.Log ("putting fortress in view");
        // arSessionOrigin.MakeContentAppearAt(content.transform, content.transform.position);
        CalcFireDist(increment);
        Debug.Log ("current: " + fireProjectile.fireSpawnDist);

    }

    private void CalcFireDist(float increment)
    {
        float fireDistDiff = GetFireSpawnDistDiff();

        if (increment < 0)
        {
            // fireDistDiff = fireProjectile.prevFireSpawnDist - fireProjectile.fireSpawnDist;
            fireProjectile.fireSpawnDist -= fireDistDiff; 
        }
        else if (increment > 0)
        {
            // fireDistDiff = fireProjectile.fireSpawnDist - fireProjectile.prevFireSpawnDist;
            fireProjectile.fireSpawnDist += fireDistDiff; 
        }
        currentFirePosition = firingPos.transform.position;
        // fireProjectile.fireSpawnDist += placeFortress.GetSessionDistance(); 
    }

    private float GetFireSpawnDistDiff()
    {
        float distance = Vector3.Distance(initFirePosition, currentFirePosition);
        Debug.Log("dist diff: " + distance);
        return distance;
    }


    

    
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFireProjectile: MonoBehaviour {
    private ARRaycastManager rays;
    public GameObject projectilePrefab;
    public Camera myCamera;
    public float cooldown, cooldownCount;
    private ARAnchorManager anc;
    private ARPlaneManager plan;
    private float appliedForce = 0.0f;
    private bool isHolding = false;
    [SerializeField] private float fireForce = 10;

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
            push();
        }
        
        if(Input.touchCount == 3) {
            logger.Log("touched 3");
            fireBlock();
        }

        if (cooldownCount > cooldown && Input.touchCount == 2) {
            fireBlock();
            cooldownCount = 0;
            logger.Log("Fired a block");
        }

        if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began && !isHolding)
                {
                    Debug.Log("Touch Pressed");
                    isHolding = true;
                }

                if (Input.touches[0].phase == TouchPhase.Ended && isHolding)
                {
                    Debug.Log("Touch Lifted/Released");
                    isHolding = false;
                    fireBlock();
                    appliedForce = 0.0f;
                }

                if(isHolding) {
                    adjustForce(appliedForce);
                    Debug.Log("Charging!");
                }
            }
    }

    public void adjustForce(float force) {
        this.appliedForce += 25.0f;
        //Adjust in a smoother way and display value on screen
        //Make force go up/down when it reaches a max/min
    }

    public void fireBlock() {  
        Vector3 screenCenter;

        screenCenter = myCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        GameObject spawnedProjectile = Instantiate(projectilePrefab, 
  			myCamera.gameObject.transform.position + 
  				(myCamera.gameObject.transform.forward * 1.0f), 
  			myCamera.gameObject.transform.rotation);	

        spawnedProjectile.GetComponent<Rigidbody>().AddForce(myCamera.gameObject.transform.forward * this.appliedForce, ForceMode.Impulse);
        spawnedProjectile.tag = "SpawnedObject";

        // logger.Log("spawned at " + spawnedProjectile.transform.position.x + ", " 
        //        + spawnedProjectile.transform.position.y + ", " + spawnedProjectile.transform.position.z);

        logger.Log("Force applied: " + this.appliedForce);
    }

    public void push () {
        RaycastHit[] myHits;
        Ray r;
                
        r = myCamera.ScreenPointToRay(Input.GetTouch(0).position);

        myHits = Physics.RaycastAll (r);

        foreach (RaycastHit hit in myHits) {
                logger.Log ("Detected " + hit.transform.gameObject.name);
                    
            if (hit.transform.gameObject.tag == "SpawnedObject") {
                logger.Log ("Applying force");
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce 
                            (r.direction * 10000);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFireProjectile: MonoBehaviour {
    private InputManager inputManager;

    private ARRaycastManager rays;
    private PlaceFortress placeFortress;
    private ScaleContent scaleContent;
    private SessionOriginController sessionController;

    public GameObject projectilePrefab;
    private Dictionary<string, GameObject> projectilePrefabs; // todo: flytta till vettigare ställe sen.

    public Camera myCamera;
    public float cooldown, cooldownCount;
    private ARAnchorManager anc;
    private ARPlaneManager plan;
    public float appliedForce = 0.0f;
    public bool isHolding = false;
    [SerializeField] private float fireForce = 10;

    [SerializeField] public float prevFireSpawnDist = 1;
    [SerializeField] public float fireSpawnDist = 1;


    private static ILogger logger = Debug.unityLogger;

    void Start() {
        cooldown = 2;
        myCamera = 
        	this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();

        rays = this.gameObject.GetComponent<ARRaycastManager>();
        anc = this.gameObject.GetComponent<ARAnchorManager>();
        plan = this.gameObject.GetComponent<ARPlaneManager>();

        placeFortress = GetComponent<PlaceFortress>();
        scaleContent = this.gameObject.GetComponent<ScaleContent>();
        sessionController = this.gameObject.GetComponent<SessionOriginController>();

        inputManager = InputManager.Instance;   
        inputManager.OnFirstTouch += CheckFirstTouchAction;
        inputManager.OnTouchCount += CheckTouchCountAction;
        
        projectilePrefabs = new Dictionary<string, GameObject>()
        {
            {"regularBlock", Resources.Load<GameObject>("Block")},
            {"explosiveBlock", Resources.Load<GameObject>("ExplosiveBlock")},
            {"iceBlock", Resources.Load<GameObject>("iceblock")}
        };
    }

    private void CheckFirstTouchAction(Touch touch)
    {
        if (GameManager.Instance.GetGameState() == GameState.PLAYING)
        {
            Vector2 screenPosition = Input.GetTouch(0).position;

            if (sessionController.IsPointOverUIObject(screenPosition))
            {   return; }   
            if (Input.touches[0].phase == TouchPhase.Began && !isHolding)
            {
                // Debug.Log("Touch Pressed");
                isHolding = true;
            }

            if (Input.touches[0].phase == TouchPhase.Ended && isHolding)
            {
                // Debug.Log("Touch Lifted/Released");
                isHolding = false;
                fireBlock();
                appliedForce = 200.0f;
                sessionController.UpdateAppliedForceText();
            }

            if(isHolding) {
                adjustForce(appliedForce);
                sessionController.UpdateAppliedForceText();
                // Debug.Log("Charging!");
            }
        }
    }
    
    private void CheckTouchCountAction(int touchCount)
    {
        if (GameManager.Instance.GetGameState() == GameState.PLAYING)
        {
            if(touchCount == 2) {
                logger.Log("touched 2");
            }

            if(touchCount == 3) {
                logger.Log("touched 3");
                fireBlock();
            }

            if (cooldownCount > cooldown && touchCount == 2) {
                fireBlock();
                cooldownCount = 0;
                logger.Log("Fired a block");
            }
        }
    } 

    public void adjustForce(float force) {
        this.appliedForce = this.appliedForce > 1000f ? 1000f : this.appliedForce + 25.0f;
        //Adjust in a smoother way and display value on screen
        //Make force go up/down when it reaches a max/min
    }

    public void fireBlock() {
        // check if player has already fired
        if (GameManager.Instance.GetPlayingState() is PlayingState.END_TURN)
            return;

        Vector3 screenCenter;

        screenCenter = myCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        GameObject spawnedProjectile = Instantiate(projectilePrefab, 
  			myCamera.gameObject.transform.position + 
  				(myCamera.gameObject.transform.forward * fireSpawnDist), 
  			myCamera.gameObject.transform.rotation);	

        spawnedProjectile.GetComponent<Rigidbody>().AddForce(myCamera.gameObject.transform.forward * this.appliedForce, ForceMode.Impulse);
        spawnedProjectile.tag = "SpawnedObject";

        // logger.Log("spawned at " + spawnedProjectile.transform.position.x + ", " 
        //        + spawnedProjectile.transform.position.y + ", " + spawnedProjectile.transform.position.z);

        // logger.Log("Force applied: " + this.appliedForce);

        GameManager.Instance.SetPlayingState(PlayingState.END_TURN);
    }

    void setProjectilePrefab(string type) {
        projectilePrefab = projectilePrefabs[type];
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

    // public void LoadAsset(string address, Action<GameObject> callback = null)
    // {
    //     Action<AsyncOperationHandle<GameObject>> addressableLoaded = (asyncOperation) =>
    //     {
    //         callback?.Invoke(asyncOperation.Result);
    //     };
    //     Addressables.LoadAssetAsync<GameObject>(address).Completed += addressableLoaded;
    // }

}

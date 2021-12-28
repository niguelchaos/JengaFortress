using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class DebugFunctions: MonoBehaviour 
{
    private ARPlaneManager planeManager;
    private ARSessionOrigin arSessionOrigin;

    private Setup setup;
    private PlaceFortress placeFortress;
    private SessionOriginController sessionController;
    private Camera myCamera;

    public GameObject debugCanvas;

    public GameObject refPlane;
    public GameObject testGroundLocation;
    public Slider debugFiringPosSlider;



    private void Awake()
    {
        arSessionOrigin = GetComponent<ARSessionOrigin>();
        planeManager = this.gameObject.GetComponent<ARPlaneManager>();
        setup = GetComponent<Setup>();
        placeFortress = GetComponent<PlaceFortress>();
        sessionController = this.gameObject.GetComponent<SessionOriginController>();

    }
    private void Start() 
    {
        myCamera = this.gameObject.transform.Find
                ("AR Camera").gameObject.GetComponent<Camera>();

        if (refPlane.activeSelf == true && planeManager.enabled)
        {
            setup.content.transform.position = refPlane.transform.position;
            // refPlane.transform.parent = arSessionOrigin.transform;
        }
        // InputManager.Instance.OnFirstTouch += CheckTouchAction;

    }

    private void Update()
    {
        if (refPlane.activeSelf == true && planeManager.enabled)
        {
            // Vector3 targetPos = new Vector3(spawnedFortress.transform.position.x, refPlane.transform.position.y, spawnedFortress.transform.position.z);
            arSessionOrigin.MakeContentAppearAt(setup.content.transform, refPlane.transform.position);
        }
    }

    public void ShowUI()
    {
        debugCanvas.SetActive(!debugCanvas.activeSelf);
    }

    public void EnableRefPlane()
    {   
        bool active = !refPlane.gameObject.activeSelf; 
        refPlane.gameObject.SetActive(active);
        sessionController.UpdateText();   
    }

    public void DisableAllGravity()
    {
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == LayerManager.BlockLayer)
            {
                SetObjectGravity(goArray[i], false);
            }
        }
    }

    public void DisableAllKinematic()
    {
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == LayerManager.BlockLayer)
            {
                SetObjectIsKinematic(goArray[i], false);
            }
        }
    }

    public void ActivateAllGravity()
    {
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == LayerManager.BlockLayer)
            {
                SetObjectGravity(goArray[i], true);
            }
        }
    }

    public void SetObjectGravity(GameObject spawnedObject, bool onOrOff)
    {
        Rigidbody[] rbs = spawnedObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = onOrOff;
        }
    }

    private void SetObjectIsKinematic(GameObject spawnedObject, bool isKOrNotIdkMan)
    {
        Rigidbody[] rbs = spawnedObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = isKOrNotIdkMan;
        }
    }

    public void PlaceTestGroundPlane()
    {
        if (testGroundLocation.activeSelf == true)
        {
            Debug.Log ("image pos:  " + testGroundLocation.transform.position);
            setup.groundPlane = Instantiate(setup.groundPlanePrefab, testGroundLocation.transform.position, Quaternion.identity);
            // placeFortress.transform.parent = testLocation.transform;
            // myGroundPlanes[key] = groundPlanePrefab;

            setup.groundPlane.AddComponent<ARAnchor>();
            // placeFortress.groundPlane.transform.parent = arOrigin.transform;

            setup.content.transform.position = setup.groundPlane.transform.position;
            Debug.Log ("groundplane pos:  " + setup.groundPlane.transform.position);
            
            
            planeManager.requestedDetectionMode = PlaneDetectionMode.None;
            planeManager.enabled = false;
            Debug.Log ("planes disabled");
            
            setup.ConfirmFinishSetup();
        }
    }

    // used by slider
    public void SetFiringPos()
    {
        setup.firingPos.transform.position = myCamera.gameObject.transform.position + (myCamera.transform.forward * debugFiringPosSlider.value);
    }
    //////////////////////////////////////////////////

    public void SetCoreBlockState()
    {
        GameManager.Instance.SetCurrentGameState(GameState.PLACE_CORE_BLOCK);
    }
    public void SetSetupState()
    {
        GameManager.Instance.SetCurrentGameState(GameState.SETUP);
    }

}
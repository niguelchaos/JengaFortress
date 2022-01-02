using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

// supposedly deals with information required by multiple classes, 
// controls stuff happening related to session
public class SessionOriginController: MonoBehaviour {

    
    public TMP_Text currentModeText;
    public TMP_Text sessionSizeText;
    public TMP_Text appliedForceText;

    private ARSessionOrigin arSessionOrigin;
    private PlaceFortress placeFortress;
    private ScaleContent scaleContent;
    private ARFireProjectile fireProjectile;

    private GameObject lastSelected;


    private void Awake()
    {
        arSessionOrigin = GetComponent<ARSessionOrigin>();
    }
    private void Start()
    {
        placeFortress = GetComponent<PlaceFortress>();
        scaleContent = this.gameObject.GetComponent<ScaleContent>();
        fireProjectile = GetComponent<ARFireProjectile>();
        UpdateText();
    }

    public bool IsPointOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else 
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
            // Debug.Log(lastSelected.name);
        }

        if (EventSystem.current.IsPointerOverGameObject()) 
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.currentSelectedGameObject.layer == LayerManager.UILayer)
                {
                    return true;
                }
            }
            return false;
        }

       PointerEventData eventPosition = new PointerEventData(EventSystem.current);
       eventPosition.position = pos;

       List<RaycastResult> results = new List<RaycastResult>();
       EventSystem.current.RaycastAll(eventPosition, results);
        
        foreach(RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null || 
                result.gameObject.GetComponent<TMP_Text>() != null)
            {
                // Debug.Log("found button ");
                return true;
            }
            else {
                Debug.Log(result.ToString());
            }
        }

        if (results.Count == 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
            // Debug.Log("just lifted");
            return true;
        }

        // Debug.Log(results.Count);
       return results.Count > 0;
    }

    public void UpdateText()
    {
        currentModeText.text = placeFortress.GetPlaceMode().ToString();
        sessionSizeText.text = arSessionOrigin.transform.localScale.ToString("F3");
    }
    public void UpdateAppliedForceText()
    {
        appliedForceText.text = fireProjectile.appliedForce.ToString();
    }
    
}
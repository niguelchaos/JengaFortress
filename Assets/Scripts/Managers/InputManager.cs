using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCodeConstants : MonoBehaviour
{
    public static readonly KeyCode FIRE = KeyCode.Space;
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // advantages of events: i guess no need to put input code inside update of each class - 
    // idk if 2+ classes polling for the same input in update is good or not 
    // let other classes define the what happens if they subscribe to FireEvent
    public delegate void FireEvent(bool fired);
    public event FireEvent OnFire;

    public delegate void TouchCountEvent(int touchCount);
    public event TouchCountEvent OnTouchCount;
    public delegate void FirstTouchEvent(Touch touch);
    public event FirstTouchEvent OnFirstTouch;
    public delegate void AllTouchesEvent(Touch[] touches);
    public event AllTouchesEvent OnAllTouches;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        GetFirePressed();
        GetTouchCounts();
        GetFirstTouch();
        GetAllTouches();
    }

    public void GetFirePressed()
    {
        bool firePressed = Input.GetKeyDown(KeyCodeConstants.FIRE);
        if (OnFire != null) 
        {   OnFire(firePressed);  }   
    }

    public void GetTouchCounts()
    {
        int touchCounts = Input.touchCount;
        if (OnTouchCount != null)
        {   OnTouchCount(touchCounts);  }
    }

    public void GetFirstTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch currentTouch = Input.GetTouch(0);
            if (OnFirstTouch != null)
            {   OnFirstTouch(currentTouch);  }
        }
    }

    public void GetAllTouches()
    {
        Touch[] allTouches = Input.touches;
        if (OnAllTouches != null)
        {   OnAllTouches(allTouches);  }
    }

}
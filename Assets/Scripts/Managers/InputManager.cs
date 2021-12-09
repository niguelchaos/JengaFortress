using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // fire event to subscribers
    // let other class define function
    public delegate void StartTouchEvent(Vector2 pos, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 pos, float time);
    public event EndTouchEvent OnEndTouch;

    private PlayerInput playerInput;
    private JengaFortress jengaFortressControls;
    private InputAction testFireAction;

    private InputAction touchPressAction;
    private InputAction touchPositionAction;

    [SerializeField] public bool testFireInput { get; set; } // TODO: make setter private (just public for testing)


    private void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
        jengaFortressControls = new JengaFortress();
    }

    // Start is called before the first frame update
    void Start()
    {
        testFireAction = playerInput.actions["testFire"];
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];

        // i couldnt get it working with playerinput
        jengaFortressControls.Player.TouchPress.started += ctx => StartTouch(ctx);
        jengaFortressControls.Player.TouchPress.canceled += ctx => EndTouch(ctx);

    }

    private void OnEnable()
    {
        jengaFortressControls.Enable();
        TouchSimulation.Enable();

        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;

    }
    private void OnDisable()
    {
        jengaFortressControls.Disable();

        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        TouchSimulation.Disable();

    }

    // Update is called once per frame
    void Update()
    {
        // // enhanced good for polling
        // Debug.Log(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches);

        // // can check what phase touch is at to respond to them
        // foreach(UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
        // {
        //     Debug.Log(touch.phase == UnityEngine.InputSystem.TouchPhase.Began);
        // }
    }

    public void onTestFireInput(InputAction.CallbackContext context)
    {
        // print("onTestFireInput:" + context.phase);

        if (context.started)
        {
            
        }
        if (context.performed)
        {
            this.testFireInput = true;
        }
        else if (context.canceled)
        {
            this.testFireInput = false;
        }
        
    }

    public void StartTouch(InputAction.CallbackContext context)
    {
        // Debug.Log("Started Touch" + jengaFortressControls.Player.TouchPosition.ReadValue<Vector2>());
        if (OnStartTouch != null)
        {
            OnStartTouch(jengaFortressControls.Player.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }
    public void EndTouch(InputAction.CallbackContext context)
    {
        // Debug.Log("Touch Ended");
        if (OnEndTouch != null)
        {
            OnEndTouch(jengaFortressControls.Player.TouchPosition.ReadValue<Vector2>(), (float)context.time);
        }
    }

    // not used
    private void FingerDown(Finger currentFinger)
    {
        if (OnStartTouch != null)
        {
            OnStartTouch(currentFinger.screenPosition, Time.time);
        }
    }



}

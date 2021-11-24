using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInput playerInput;
    private InputAction testFireAction;

    [SerializeField] public bool testFireInput { get; set; } // TODO: make setter private (just public for testing)


    private void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        testFireAction = playerInput.actions["testFire"];
        //testFireAction.performed += x => onTestFireInput(x);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onTestFireInput(InputAction.CallbackContext context)
    {
        print("onTestFireInput:" + context.phase);

        if (context.started)
        {
            
        }
        if (context.performed)
        {
            this.testFireInput = true;
        }
        else if (context.canceled)
        {
            //this.testFireInput = false;
        }
        
    }

}

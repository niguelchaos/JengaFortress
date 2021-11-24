using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private GameObject projectile;    // this is a reference to your projectile prefab

    private InputAction testFireAction;

    [SerializeField] private bool hasFired = false;
    [SerializeField] private float fireForce = 10000;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        Fire();
    }

    void Fire()
    {
        if (InputManager.Instance.testFireInput && !hasFired)
        {
            Transform spawnTransform = gameObject.transform;
            
            projectile = Instantiate(projectile, spawnTransform.position, spawnTransform.rotation);
            projectile.GetComponent<Rigidbody>().AddForce(spawnTransform.forward * fireForce, ForceMode.Impulse);

            // button held down, fired already
            hasFired = true;
            // InputManager.Instance.testFireInput = false; // TEMP
            // print("fire");
        }
    }

    private void ProcessInput()
    {
        if (InputManager.Instance.testFireInput == false)
        {
            // button lifted
            hasFired = false;
        }
    }

}

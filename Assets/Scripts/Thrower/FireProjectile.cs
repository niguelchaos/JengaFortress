using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private GameObject projectile;    // this is a reference to your projectile prefab

    private InputAction testFireAction;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Fire();
    }

    void Fire()
    {
        if (InputManager.Instance.testFireInput)
        {
            Transform spawnTransform = gameObject.transform;
            
            projectile = Instantiate(projectile, spawnTransform.position, spawnTransform.rotation);
            projectile.GetComponent<Rigidbody>().AddForce(spawnTransform.forward * 100000);

            InputManager.Instance.testFireInput = false; // TEMP
            print("fire");
        }
    }

}

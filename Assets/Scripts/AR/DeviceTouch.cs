using UnityEngine;

public class DeviceTouch : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject projectile;    // this is a reference to your projectile prefab

    private GameObject spawnedObject;

    private void Start()
    {
        InputManager.Instance.OnFirstTouch += Spawn;
        InputManager.Instance.OnFire += ActivateAllPhysics;
    }

    public void Spawn(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            Vector3 screenCoordinates = new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane);
            Vector3 worldCoords = mainCamera.ScreenToWorldPoint(screenCoordinates);
            // worldCoords.z = 0;
            // transform.position = worldCoords;

            Vector3 spawnPos = worldCoords;
            // print(spawnPos);
            GameObject spawnedProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
            spawnedObject = spawnedProjectile;
            if (spawnedObject != null)
            {
                // SetObjectGravity(spawnedProjectile, false);
                SetObjectIsKinematic(spawnedObject, true); 
            }
        }
    }
    private void ActivateAllPhysics(bool fired)
    {
        if (fired)
        {
            if (spawnedObject != null)
            {
                SetObjectIsKinematic(spawnedObject, false);
                SetObjectGravity(spawnedObject, true);
            }
        }
    }

    public void SetObjectGravity(GameObject spawnedObject, bool waw)
    {
        Rigidbody[] rbs = spawnedObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            Debug.Log(spawnedObject.name);
            Debug.Log(rb.gameObject.name);
            rb.useGravity = waw;
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

    public void ActivatePhysics()
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

    

    
}
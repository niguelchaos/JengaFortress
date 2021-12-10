using UnityEngine;

public class DeviceTouch : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject projectile;    // this is a reference to your projectile prefab


    // private void OnEnable()
    // {
    //     newInputSysManager.OnStartTouch += Spawn;
    // }
    // private void OnDisable()
    // {
    //     newInputSysManager.OnStartTouch -= Spawn;
    // }

    public void Spawn(Vector2 screenPosition, float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane);
        Vector3 worldCoords = mainCamera.ScreenToWorldPoint(screenCoordinates);
        // worldCoords.z = 0;
        // transform.position = worldCoords;

        Vector3 spawnPos = worldCoords;
        print(spawnPos);
        GameObject spawnedProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
    }

    
}
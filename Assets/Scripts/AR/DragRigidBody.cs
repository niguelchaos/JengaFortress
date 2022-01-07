using UnityEngine;

public class DragRigidBody : MonoBehaviour
{
    public float forceAmount = 100;

    Rigidbody selectedRigidbody;
    Camera targetCamera;
    Vector3 originalScreenTargetPosition;
    Vector3 originalRigidbodyPos;
    float selectionDistance;

    // Start is called before the first frame update
    void Start()
    {
        targetCamera = this.gameObject.transform.Find
            ("AR Camera").gameObject.GetComponent<Camera>();
        // fireProjectile = this.gameObject.transform.GetComponent<ARFireProjectile>();

        InputManager.Instance.OnFirstTouch += CheckFirstTouchAction;
    }

    void Update()
    {
        if (!targetCamera)
            return;

        // if (Input.GetMouseButtonDown(0))
        // {
        //     //Check if we are hovering over Rigidbody, if so, select it
        //     selectedRigidbody = GetRigidbodyFromMouseClick();
        // }
        // if (Input.GetMouseButtonUp(0) && selectedRigidbody)
        // {
        //     //Release selected Rigidbody if there any
        //     selectedRigidbody = null;
        // }
    }

    private void CheckFirstTouchAction(Touch touch)
    {
        if (GameManager.Instance.GetGameState() == GameState.PLAYING
            && GameManager.Instance.coreLoopMode == CoreLoopMode.MOVE_BLOCK)
        {
            if (touch.phase == TouchPhase.Began)
            {
                //Check if we are hovering over Rigidbody, if so, select it
                selectedRigidbody = GetRigidbodyFromMouseClick(touch);
            }
            if (touch.phase == TouchPhase.Ended && selectedRigidbody)
            {
                //Release selected Rigidbody if there any
                selectedRigidbody = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (selectedRigidbody)
        {
            Vector3 mousePositionOffset = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance)) - originalScreenTargetPosition;
            selectedRigidbody.velocity = (originalRigidbodyPos + mousePositionOffset - selectedRigidbody.transform.position) * forceAmount * Time.deltaTime;
        }
    }

    Rigidbody GetRigidbodyFromMouseClick(Touch touch)
    {
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = targetCamera.ScreenPointToRay(touch.position);
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        if (hit)
        {
            if (hitInfo.collider.gameObject.GetComponent<Rigidbody>())
            {
                if (!hitInfo.collider.gameObject.CompareTag("CoreBlock"))
                {
                    selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
                    originalScreenTargetPosition = targetCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, selectionDistance));
                    originalRigidbodyPos = hitInfo.collider.transform.position;
                    return hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                }
            }
        }
        return null;
    }
}
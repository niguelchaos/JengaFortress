using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    [SerializeField] private Player player;
    private BoxCollider boundaryCollider;

    // The objects that are in the player's boundary
    private string p1GameObjName = "Fortress_P1";
    private string p2GameObjName = "Fortress_P2";

    [SerializeField] private GameObject playerObjects;
    private MeshRenderer meshRenderer;

    void Start()
    {
        // TODO: Should the boundaries be dynamically sized?
        boundaryCollider = GetComponent<BoxCollider>();

        meshRenderer = GetComponent<MeshRenderer>();


    }

    
    
    private void AttachToStartingPoint()
    {
        //transform.position = 
    }


}

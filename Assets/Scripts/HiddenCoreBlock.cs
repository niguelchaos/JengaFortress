using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HiddenBlockPlayer {P1, P2}

public class HiddenCoreBlock : MonoBehaviour
{
    private HiddenBlockPlayer player;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerManager.GroundLayer)
        {
            Debug.Log("hit ground");
            GameManager.Instance.SetCurrentGameState(GameState.GAME_OVER);
        }
    }

    public HiddenBlockPlayer GetPlayer() {  return player; }
}

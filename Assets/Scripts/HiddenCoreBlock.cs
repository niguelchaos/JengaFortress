using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HiddenBlockPlayer {P1, P2}

public class HiddenCoreBlock : MonoBehaviour
{
    [SerializeField] private HiddenBlockPlayer player;
    private Rigidbody rb;
    private Outline outline;


    private void Awake()
    {
        // run update on game state change
        outline = this.GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();

        // subscribe to state changes
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
    }


    private void Update()
    {
        // temporarily here for debug
        // CheckOutline();
    }

    private void UpdateOnGameStateChanged(GameState currentGameState)
    {
        CheckOutline();
    }

    private void OnDestroy()
    {
        // unsubscribe if destroy obj
        GameManager.OnGameStateChanged -= UpdateOnGameStateChanged;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerManager.GroundLayer)
        {
            Debug.Log("hit ground");
            GameManager.Instance.SetCurrentGameState(GameState.GAME_OVER);
        }
    }

    // show player's outlined hidden block only on their turn. 
    public void CheckOutline()
    {
        DisableOutline();
        switch (GameManager.Instance.GetCurrentGameState(), player)
        {
            case (GameState.PLAYER_1, HiddenBlockPlayer.P1):
            case (GameState.PLAYER_2, HiddenBlockPlayer.P2):
                EnableOutline();
                break;
        }
    }

    public HiddenBlockPlayer GetPlayer() {  return player; }
    public void SetPlayer(HiddenBlockPlayer newPlayer)
    {   this.player = newPlayer;    }

    public void EnableOutline()     {   outline.enabled = true;  }
    public void DisableOutline()    {   outline.enabled = false; }
}

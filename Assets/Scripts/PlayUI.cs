using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayUI : MonoBehaviour
{

    public TMP_Text currentPlayerText;
    //private Player player;

    void Start()
    {
        UpdateText();
        GameManager.OnPlayingStateChanged += UpdateOnPlayingStateChanged;
    }

    private void UpdateText()
    {
        currentPlayerText.text = "Player " + (int) GameManager.Instance.currentPlayer;
    }

    public void ChangePlayer()
    {
        GameManager.Instance.ChangePlayer();
        UpdateText();
    }

    private void UpdateOnPlayingStateChanged(PlayingState playingState)
    {
        if(playingState is PlayingState.END_TURN)
            SetEndTurnButtonActive(true);
        else
            SetEndTurnButtonActive(false);
    }

    public void SetEndTurnButtonActive(bool active)
    {
        GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = active;
    }
}

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
    }

    private void UpdateText()
    {
        //currentPlayerText.text = "Player " + (int)GameManager.Instance.GetCurrentPlayer();
        currentPlayerText.text = "Player " + GameManager.Instance.currentPlayer;
    }

    public void ChangePlayer()
    {
        GameManager.Instance.ChangePlayer();
        UpdateText();
    }
}

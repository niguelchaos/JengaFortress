using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayUI : MonoBehaviour
{

    public TMP_Text currentPlayerText;
    public Button endTurnButton;
    public Image endTurnButtonImage;
    //private Player player;

    void Start()
    {
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButtonImage = GameObject.Find("EndTurnButton").GetComponent<Image>();
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
        endTurnButton.interactable = active;
        if (active)
            StartCoroutine("startBlinkButton");
        else
            StopCoroutine("startBlinkButton");
    }

    private IEnumerator startBlinkButton()
    {
        while (true)
        {
            endTurnButtonImage.color = new Color(0.7f, 0.85f, 0.75f, 1f);
            yield return new WaitForSeconds(1.5f);
            endTurnButtonImage.color = new Color(0.75f, 0.90f, 0.8f, 1f);
            yield return new WaitForSeconds(1.5f);
        }
    }
}

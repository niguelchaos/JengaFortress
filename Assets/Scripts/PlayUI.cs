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
    public Canvas oldCanvas;
    public Canvas startScreen;
    public Canvas placeGroundScreen;
    public Canvas adjustFireScreen;
    public Canvas placePlayer1Screen;
    public Canvas placePlayer2Screen;
    public Canvas playingScreen;
    //private Player player;

    void Start()
    {
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButtonImage = GameObject.Find("EndTurnButton").GetComponent<Image>();
        currentPlayerText = GameObject.Find("CurrentPlayerText").GetComponent<TMP_Text>();

        oldCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        startScreen = GameObject.Find("startScreen").GetComponent<Canvas>();
        placeGroundScreen = GameObject.Find("placeGroundScreen").GetComponent<Canvas>();
        adjustFireScreen = GameObject.Find("adjustFireScreen").GetComponent<Canvas>();
        placePlayer1Screen = GameObject.Find("placePlayer1").GetComponent<Canvas>();
        placePlayer2Screen = GameObject.Find("placePlayer2").GetComponent<Canvas>();
        playingScreen = GameObject.Find("playingScreen").GetComponent<Canvas>();
        UpdateText();
        GameManager.OnPlayingStateChanged += UpdateOnPlayingStateChanged;

        oldCanvas.gameObject.SetActive(false);
        startScreen.gameObject.SetActive(true);
        placeGroundScreen.gameObject.SetActive(false);
        adjustFireScreen.gameObject.SetActive(false);
        placePlayer1Screen.gameObject.SetActive(false);
        placePlayer2Screen.gameObject.SetActive(false);
        playingScreen.gameObject.SetActive(false);
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

    public void activatePlaceGround(){
        switchScreen(startScreen, placeGroundScreen);
    }

    public void activateAdjustFireScreen(){
        switchScreen(placeGroundScreen, adjustFireScreen);
    }

    public void activatePlacePlayer1() {
        switchScreen(adjustFireScreen, placePlayer1Screen);
    }

    public void activatePlacePlayer2() {
        switchScreen(placePlayer1Screen, placePlayer2Screen);
    }

    public void activatePlayingScreen() {
        switchScreen(placePlayer2Screen, playingScreen);
    }

    private void switchScreen(Canvas a, Canvas b) {
        a.gameObject.SetActive(false);
        b.gameObject.SetActive(true);
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
